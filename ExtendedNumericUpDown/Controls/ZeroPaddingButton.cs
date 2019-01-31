namespace ExtendedNumericUpDown.Controls
{
    using System.Drawing;
    using System.Windows.Forms;

    public sealed class ZeroPaddingButton : Button
    {
        public ZeroPaddingButton() : base()
        {
            this.FlatStyle = FlatStyle.Flat;

            this.BackColor = Color.Red;
            this.ForeColor = Color.White;

            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Font = new Font(new Font("Arial", 9), FontStyle.Bold);

            this.MouseEnter += (sender, args) => this.BackColor = this.MouseEnterColor;
            this.MouseLeave += (sender, args) => this.BackColor = this.MouseLeaveColor;
        }

        public Color MouseEnterColor { get; set; }
        public Color MouseLeaveColor { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            using (var backColorPen = new Pen(BackColor))
            using (var foreColorPen = new Pen(ForeColor))
            {
                paintEventArgs.Graphics.FillRectangle(backColorPen.Brush, ClientRectangle);

                paintEventArgs.Graphics.DrawString(this.Text, Font, foreColorPen.Brush, new PointF(this.Size.Width / 2 - 6, 1));
                paintEventArgs.Graphics.DrawRectangle(foreColorPen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }
    }
}