namespace ExtendedNumericUpDown.Controls
{
    partial class ExtendedNumericUpDown
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainPanel = new System.Windows.Forms.Panel();
            this.DecrementButton = new ZeroPaddingButton();
            this.IncrementButton = new ZeroPaddingButton();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.validationErrorLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.DecrementButton);
            this.MainPanel.Controls.Add(this.IncrementButton);
            this.MainPanel.Location = new System.Drawing.Point(0, 18);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(136, 33);
            this.MainPanel.TabIndex = 3;
            // 
            // DecrementButton
            // 
            this.DecrementButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DecrementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DecrementButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.DecrementButton.ForeColor = System.Drawing.Color.White;
            this.DecrementButton.Location = new System.Drawing.Point(99, 15);
            this.DecrementButton.MouseEnterColor = System.Drawing.Color.Gray;
            this.DecrementButton.MouseLeaveColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DecrementButton.Name = "DecrementButton";
            this.DecrementButton.Size = new System.Drawing.Size(36, 17);
            this.DecrementButton.TabIndex = 1;
            this.DecrementButton.Text = " ̶ ";
            this.DecrementButton.UseVisualStyleBackColor = false;
            this.DecrementButton.Click += new System.EventHandler(this.OnDecrementButtonClick);
            this.DecrementButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnDecrementButtonMouseUp);
            this.DecrementButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnDecrementButtonMouseDown);
            // 
            // IncrementButton
            // 
            this.IncrementButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.IncrementButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IncrementButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.IncrementButton.ForeColor = System.Drawing.Color.White;
            this.IncrementButton.Location = new System.Drawing.Point(99, 1);
            this.IncrementButton.MouseEnterColor = System.Drawing.Color.Gray;
            this.IncrementButton.MouseLeaveColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.IncrementButton.Name = "IncrementButton";
            this.IncrementButton.Size = new System.Drawing.Size(36, 16);
            this.IncrementButton.TabIndex = 0;
            this.IncrementButton.Text = "+";
            this.IncrementButton.UseVisualStyleBackColor = false;
            this.IncrementButton.Click += new System.EventHandler(this.OnIncrementButtonClick);
            this.IncrementButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnIncrementButtonMouseDown);
            this.IncrementButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnIncrementButtonMouseUp);
            // 
            // TextBox
            // 
            this.TextBox.BackColor = System.Drawing.Color.White;
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox.Font = new System.Drawing.Font("Bahnschrift Condensed", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TextBox.ForeColor = System.Drawing.Color.Black;
            this.TextBox.Location = new System.Drawing.Point(1, 20);
            this.TextBox.MaxLength = 13;
            this.TextBox.Multiline = true;
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(98, 29);
            this.TextBox.TabIndex = 4;
            this.TextBox.Text = "1000,12345678";
            this.TextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
            this.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTextBoxKeyDown);
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnTextBoxKeyUp);
            // 
            // validationErrorLabel
            // 
            this.validationErrorLabel.BackColor = System.Drawing.Color.White;
            this.validationErrorLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.validationErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.validationErrorLabel.Location = new System.Drawing.Point(1, 53);
            this.validationErrorLabel.Name = "validationErrorLabel";
            this.validationErrorLabel.Size = new System.Drawing.Size(135, 37);
            this.validationErrorLabel.TabIndex = 4;
            this.validationErrorLabel.Text = "Incorrect value.";
            this.validationErrorLabel.Visible = false;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TitleLabel.ForeColor = System.Drawing.Color.Black;
            this.TitleLabel.Location = new System.Drawing.Point(-1, 2);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(45, 13);
            this.TitleLabel.TabIndex = 5;
            this.TitleLabel.Text = "Volume:";
            // 
            // ExtendedNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.validationErrorLabel);
            this.Controls.Add(this.MainPanel);
            this.Name = "ExtendedNumericUpDown";
            this.Size = new System.Drawing.Size(136, 90);
            this.Load += new System.EventHandler(this.OnLoad);
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.TextBox TextBox;

        private System.Windows.Forms.Label validationErrorLabel;
        private System.Windows.Forms.Label TitleLabel;
        private ZeroPaddingButton DecrementButton;
        private ZeroPaddingButton IncrementButton;
    }
}
