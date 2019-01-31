namespace ExtendedNumericUpDown.Controls
{
    using global::ExtendedNumericUpDown.Models;

    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Windows.Forms;
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public partial class ExtendedNumericUpDown : UserControl
    {
        #region PrivateFields

        private decimal _minimumValue = 0M;
        private decimal _maximumValue = decimal.MaxValue;

        private decimal _step = 0.01M;
        private decimal _value;

        private bool _isValid = true;

        private ExtendedCancellationTokenSource _continuousIncrementOperationCts;
        private ExtendedCancellationTokenSource _continuousDecrementOperationCts;

        #endregion

        #region Constructors

        public ExtendedNumericUpDown()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        #endregion

        #region Properties

        public string Title
        {
            get { return this.TitleLabel.Text; }
            set
            {
                this.TitleLabel.Text = value;
            }
        }

        public new string Text
        {
            get { return this.TextBox.Text; }
            set
            {
                this.TextBox.Text = value;
            }
        }

        public int DecimalPlaces { get; set; } = 2;

        public decimal Value
        {
            get { return _value; }
            set
            {
                this.InternalValue = value;
                this.UpdateDisplayValue();
            }
        }

        private decimal InternalValue
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    this.RefreshButtonsState();
                    this.ValueChanged?.Invoke(this, null);
                }
            }
        }

        public decimal Minimum
        {
            get { return _minimumValue; }
            set
            {
                if (value != _minimumValue)
                {
                    _minimumValue = value;
                    this.HandleExtremumsChanged();
                }
            }
        }

        public decimal Maximum
        {
            get { return _maximumValue; }
            set
            {
                if (value != _maximumValue)
                {
                    _maximumValue = value;
                    this.HandleExtremumsChanged();
                }
            }
        }

        public decimal Increment
        {
            get { return _step; }
            set
            {
                if (value != _step)
                {
                    _step = value;
                    this.UpdateDisplayValue();
                }
            }
        }

        public bool IsValid
        {
            get { return !this.Enabled || _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;

                    if (_isValid)
                    {
                        this.RefreshButtonsState();
                    }
                    else
                    {
                        this.IncrementButton.Enabled = this.DecrementButton.Enabled = false;
                    }

                    this.ValueChanged?.Invoke(this, null);
                }
            }
        }

        public bool NoMaximum { get; set; }

        public bool EnableValidationClient { get; set; } = true;

        protected CultureInfo CurrentInputCulture => InputLanguage.CurrentInputLanguage.Culture;

        protected char CurrentDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

        #endregion

        #region EventHandlers

        private void OnLoad(object sender, EventArgs e)
        {
            this.SubscribeForEvents();  
            this.UpdateTextBoxFlowDirection();
            this.UpdateDisplayValue();
        }

        private void OnInputLanguageChanged(object sender, InputLanguageChangedEventArgs e) => this.UpdateTextBoxFlowDirection();

        private void OnIncrementButtonClick(object sender, EventArgs e)
        {
            if (!this.IsContinuousOperationAlive())
            {
                this.TryIncrementValue();
            }
        }

        private void OnDecrementButtonClick(object sender, EventArgs e)
        {
            if (!this.IsContinuousOperationAlive())
            {
                this.TryDecrementValue();
            }
        }

        #region ContinuousIncrementHandling

        private async void OnIncrementButtonMouseDown(object sender, MouseEventArgs e)
        {
            // Cancel all CancellationTokens & dispose them:
            this.CancelAllContinuousUnaryOperationCts();

            // Initialize new CancellationTokenSource for coordinated cancellation:
            _continuousIncrementOperationCts = new ExtendedCancellationTokenSource();

            try
            {
                // Start continuous value changing operation:
                await this.StartContinuousUnaryValueChangingAsync(_continuousIncrementOperationCts.Token, this.TryIncrementValue);
            }
            catch (TaskCanceledException)
            {
                _continuousIncrementOperationCts?.SafeCancelAndDispose();
            }
        }

        private void OnIncrementButtonMouseUp(object sender, MouseEventArgs e) => _continuousIncrementOperationCts?.SafeCancelAndDispose();

        #endregion

        #region ContinuousDecrementHandling

        private async void OnDecrementButtonMouseDown(object sender, MouseEventArgs e)
        {
            // Cancel all CancellationTokens & dispose them:
            this.CancelAllContinuousUnaryOperationCts();

            // Initialize new CancellationTokenSource for coordinated cancellation:
            _continuousDecrementOperationCts = new ExtendedCancellationTokenSource();

            try
            {
                // Start continuous value changing operation:
                await this.StartContinuousUnaryValueChangingAsync(_continuousDecrementOperationCts.Token, this.TryDecrementValue);
            }
            catch (TaskCanceledException)
            {
                _continuousDecrementOperationCts?.SafeCancelAndDispose();
            }
        }

        private void OnDecrementButtonMouseUp(object sender, MouseEventArgs e) => _continuousDecrementOperationCts?.SafeCancelAndDispose();

        #endregion

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = (int)e.KeyCode;

            var currentCaretIndex = this.TextBox.SelectionStart;

            var decimalSeparatorIndex = this.Text.IndexOf(this.CurrentDecimalSeparator);

            // Suppress leading zeros duplication (not decimals):
            if ((keyCode == (int)Keys.D0 || keyCode == (int)Keys.NumPad0) && this.Text == "0")
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Suppress invalid separator:
            if (this.CurrentDecimalSeparator == '.' && 
                (e.Modifiers == Keys.Shift && keyCode == (int)Keys.Oem2 ||
                 e.Modifiers == Keys.Shift && keyCode == (int)Keys.Oemcomma ||
                 keyCode == (int)Keys.Oemcomma ||
                 keyCode == (int)Keys.Oem7) ||

                this.CurrentDecimalSeparator == ',' &&
                (keyCode == (int)Keys.Oem2 ||
                 e.Modifiers == Keys.Shift && keyCode == (int)Keys.OemPeriod ||
                 keyCode == (int)Keys.OemPeriod ||
                 keyCode == (int)Keys.Decimal))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Suppress leading zeros duplication (decimals):
            if ((keyCode == (int)Keys.D0 || keyCode == (int)Keys.NumPad0) && 
                this.Text.Length > 0 && (this.Text[0] == '0' && currentCaretIndex <= decimalSeparatorIndex || 
                this.Text[0] != '0' && this.Text[0] != this.CurrentDecimalSeparator && currentCaretIndex == 0) &&
                !this.IsWholeTextSelected())
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return; 
            }

            // Restrict decimal places:
            if (decimalSeparatorIndex != -1 && currentCaretIndex >= decimalSeparatorIndex &&
                (keyCode >= (int)Keys.D0 && keyCode <= (int)Keys.D9 || // Numerics.
                 keyCode >= (int)Keys.NumPad0 && keyCode <= (int)Keys.NumPad9)) // NumPad numerics . 
            {
                var splittedText = this.Text.Split(this.CurrentDecimalSeparator);
                if (splittedText.Length == 2 &&
                    splittedText[0].Length > 0 &&
                    splittedText[1].Length == this.DecimalPlaces &&
                    currentCaretIndex > decimalSeparatorIndex)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            // Suppress decimal separator duplication:
            if (this.IsSeparatorEntered(e) && this.Text.Contains(this.CurrentDecimalSeparator.ToString()))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            // Handle increment:
            if (keyCode == (int)Keys.Up) // UpArrow.
            {
                this.TryIncrementValue();
                return;
            }

            // Handle decrement:
            if (keyCode == (int)Keys.Down) // DownArrow.
            {
                this.TryDecrementValue();
                return;
            }

            // Suppress other invalid keys:
            if (!(this.IsSeparatorEntered(e) || /* Automatic decimal separators.
                  
                  Numeric buttons: */
                  keyCode >= (int)Keys.D0 && keyCode <= (int)Keys.D9 || // Numerics.
                  keyCode >= (int)Keys.NumPad0 && keyCode <= (int)Keys.NumPad9 || /* NumPad numerics.
                 
                  Other buttons: */
                  keyCode == (int)Keys.Left || keyCode == (int)Keys.Right || // Left & Right arrows.
                  keyCode == (int)Keys.Back || keyCode == (int)Keys.Delete || /* Backspace & Delete.
                
                  Shortcuts checks: */
                  e.Shift && keyCode == (int)Keys.Back || // Backspace + Delete.
                  e.Shift && keyCode == (int)Keys.Delete)) // Shift + Delete.
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;

            if ((keyCode == Keys.Back || keyCode == Keys.Delete) && this.Text.Length > 0)
            {
                var leadingZerosCount = 0;

                for (var i = 0; i < this.Text.Length && this.Text[i] == '0'; i++, leadingZerosCount++)
                {
                    /* Empty body. This loop is for counting only. */
                }

                if (leadingZerosCount > 0 && leadingZerosCount != this.Text.Length && 
                    !this.Text.Contains(this.CurrentDecimalSeparator.ToString()))
                {
                    this.Text = this.Text.TrimStart('0');
                    this.MoveCaretToEnd();
                }

                if (this.Text.Length > 1 && this.Text.All(_ => _ == '0'))
                {
                    this.Text = "0";
                    this.MoveCaretToEnd();
                }
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!this.EnableValidationClient)
            {
                this.IsValid = true;
                return;
            }

            if (this.IsTextValid())
            {
                this.InternalValue = decimal.Parse(this.Text);

                if (this.InternalValue < _minimumValue || this.InternalValue > _maximumValue)
                {
                    this.IsValid = false;

                    this.validationErrorLabel.Text = !this.NoMaximum ? $"Value must be in [{_minimumValue} : {_maximumValue}]"
                                                                     : $"Value must be greater or equal to {_minimumValue}]";
                    validationErrorLabel.Visible = true;
                }
                else
                {
                    this.IsValid = true;

                    validationErrorLabel.Visible = false;
                }
            }
            else
            {
                this.IsValid = false;
                this.validationErrorLabel.Text = @"Incorrect value.";
                validationErrorLabel.Visible = true;
            }
        }

        #endregion

        #region Disposing

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                this.UnsubscribeFromEvents();
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region PrivateMethods

        private void TryIncrementValue()
        {
            if (this.InternalValue + _step <= _maximumValue)
            {
                this.InternalValue += _step;
                this.UpdateDisplayValue();
                this.MoveCaretToEnd();
            }
        }

        private void TryDecrementValue()
        {
            if (this.InternalValue - _step >= _minimumValue)
            {
                this.InternalValue -= _step;
                this.UpdateDisplayValue();
                this.MoveCaretToEnd();
            }
        }

        private void MoveCaretToEnd()
        {
            this.TextBox.SelectionStart = this.TextBox.Text.Length;
        }

        private void RefreshButtonsState()
        {
            this.IncrementButton.Enabled = this.InternalValue != _maximumValue;
            this.DecrementButton.Enabled = this.InternalValue != _minimumValue;
        }

        private void UpdateDisplayValue()
        {
            this.Text = this.InternalValue.ToString($"0.{new string('0', this.DecimalPlaces)}")
                                          .Replace('.', this.CurrentDecimalSeparator);
        }

        private bool IsTextValid()
        {
            if (string.IsNullOrEmpty(this.Text)) return false;

            var stringifiedDecimalSeparator = this.CurrentDecimalSeparator.ToString();

            if (this.Text.StartsWith(stringifiedDecimalSeparator) || this.Text.EndsWith(stringifiedDecimalSeparator))
            {
                return false;
            }

            decimal value;

            return decimal.TryParse(this.Text, out value);
        }

        private void HandleExtremumsChanged()
        {
            if (this.InternalValue < _minimumValue || this.InternalValue > _maximumValue)
            {
                this.InternalValue = _minimumValue;
                this.UpdateDisplayValue();
            }
        }

        private bool IsSeparatorEntered(KeyEventArgs e)
        {
            var result = this.CurrentDecimalSeparator == '.' && e.KeyCode == Keys.Decimal;

            var loweredCurrentCultureName = this.CurrentInputCulture.Name.ToLowerInvariant();

            // Current input layout resolving:
            if (loweredCurrentCultureName.Equals("ru-RU", StringComparison.OrdinalIgnoreCase) ||
                loweredCurrentCultureName.Equals("uk-UA", StringComparison.OrdinalIgnoreCase) ||
                loweredCurrentCultureName.Equals("be-BY", StringComparison.OrdinalIgnoreCase))
            {
                result = this.CurrentDecimalSeparator == '.' ? e.KeyCode == Keys.Oem2
                                                             : e.KeyCode == Keys.Oem2 && e.Modifiers == Keys.Shift;
#if DEBUG
                if (result)
                {
                    System.Diagnostics.Debug.WriteLine(e.KeyData.ToString());
                }
#endif
            }
            else if (loweredCurrentCultureName.Contains("ar-") ||
                     loweredCurrentCultureName.Contains("arab"))
            {
                result = this.CurrentDecimalSeparator == '.' ? e.Modifiers == Keys.Shift && e.KeyCode == Keys.OemPeriod
                                                             : e.Modifiers == Keys.Shift && e.KeyCode == Keys.Oemcomma;
            }
            else if (loweredCurrentCultureName.Contains("he-"))
            {
                result = this.CurrentDecimalSeparator == '.' ? e.KeyCode == Keys.Oem2
                                                             : e.KeyCode == Keys.Oem7;
            }
            else
            {
                result = CurrentDecimalSeparator == '.' ? e.KeyCode == Keys.OemPeriod
                                                        : e.KeyCode == Keys.Oemcomma;
            }

            return result;
        }

        private void UpdateTextBoxFlowDirection()
        {
            this.TextBox.RightToLeft = this.CurrentInputCulture.TextInfo.IsRightToLeft ? RightToLeft.Yes
                                                                                       : RightToLeft.No;
        }

        private void SubscribeForEvents()
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.InputLanguageChanged += this.OnInputLanguageChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.InputLanguageChanged -= this.OnInputLanguageChanged;
            }
        }

        private bool IsWholeTextSelected() => this.TextBox.SelectedText == this.TextBox.Text;

        private async Task StartContinuousUnaryValueChangingAsync(CancellationToken cancellationToken, Action unaryOperationAction)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                await Task.CompletedTask;
            }

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await Task.CompletedTask;
                }

                // Invoke an action at correct SynchronizationContext / main thread:
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(unaryOperationAction));
                }
                else
                {
                    unaryOperationAction.Invoke();
                }

                // Delay for counter eye-view:
                await Task.Delay(125, cancellationToken);
            }
        }

        private void CancelAllContinuousUnaryOperationCts()
        {
            if (!(_continuousIncrementOperationCts == null || _continuousIncrementOperationCts.IsCancellationRequested))
            {
                _continuousIncrementOperationCts?.SafeCancelAndDispose();
            }

            if (!(_continuousDecrementOperationCts == null || _continuousDecrementOperationCts.IsCancellationRequested))
            {
                _continuousDecrementOperationCts?.SafeCancelAndDispose();
            }
        }

        private bool IsContinuousOperationAlive()
        {
            return !(_continuousIncrementOperationCts == null || _continuousIncrementOperationCts.IsCancellationRequested) ||
                   !(_continuousDecrementOperationCts == null || _continuousDecrementOperationCts.IsCancellationRequested);
        }

        #endregion
    }
}