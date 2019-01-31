namespace ExtendedNumericUpDown.Models
{
    using System.Threading;
    using System;

    public class ExtendedCancellationTokenSource : IDisposable
    {
        #region Constructor

        public ExtendedCancellationTokenSource()
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CreationDateTime = DateTime.UtcNow;
        }

        #endregion

        #region Properties

        public CancellationTokenSource CancellationTokenSource { get; }

        public CancellationToken Token => this.CancellationTokenSource?.Token ?? default(CancellationToken);

        public bool IsCancellationRequested => this.CancellationTokenSource.IsCancellationRequested;

        public DateTime CreationDateTime { get; }

        #endregion

        #region PublicMethods

        public void SafeCancelAndDispose()
        {
            try
            {
                this.CancellationTokenSource?.Cancel();
                this.Dispose();
            }
            catch (ObjectDisposedException)
            {
                /* Ignored. */
            }
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    this.CancellationTokenSource?.Dispose();
                }
                catch (Exception ex) when (ex is NullReferenceException || ex is ObjectDisposedException)
                {
                    /* Ignored. (When requested resource has already been disposed). */
                }
            }
        }

        #endregion
    }
}