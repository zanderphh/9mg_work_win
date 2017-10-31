using System.Drawing;
using System.Windows.Forms;

namespace JunZhe.O2O.SysUpdate
{
    public class BaseProgressBar : ProgressBar
    {
        public BaseProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics,
            //      this.ClientRectangle,
            //      ColorTranslator.FromHtml("#B9C0C7"), 0, ButtonBorderStyle.Solid, //左
            //      ColorTranslator.FromHtml("#B9C0C7"), 0, ButtonBorderStyle.Solid, //上
            //      ColorTranslator.FromHtml("#B9C0C7"), 0, ButtonBorderStyle.Solid, //右
            //      ColorTranslator.FromHtml("#B9C0C7"), 0, ButtonBorderStyle.Solid); //下

            SolidBrush brush = new SolidBrush(Color.Red);
            Rectangle rec = new Rectangle(-1, -1, this.Width, this.Height);

            if (ProgressBarRenderer.IsSupported)
            {
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);
            }

            Pen pen = new Pen(brush, 2);
            e.Graphics.DrawRectangle(pen, rec);

            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 2, 2, rec.Width - 4, rec.Height - 4);

            rec.Height -= 4;
            rec.Width = (int)(rec.Width*((double)Value/Maximum))-4;
            brush = new SolidBrush(this.ForeColor);
            e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);


        }
    }
}