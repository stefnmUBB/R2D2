using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R2D2.Graphing2D
{
    public partial class Path2DControl : UserControl
    {
        public Func<float, PointF> Func;
        public PointF MinPoint=Point.Empty, MaxPoint=Point.Empty;
        public float TMin = 0, TMax = 1, dT = 0.1f;
        public Color HorizAxisColor, VertAxisColor;
        public string CaptionText { get => Caption.Text; set => Caption.Text = value; }
        public Path2DControl()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {            
            var w = MaxPoint.X - MinPoint.X;
            var h = MaxPoint.Y - MinPoint.Y;
            if (w <= 0 || h <= 0) return;
            var bmp = new Bitmap(Width, Height);         
            using (Graphics g = Graphics.FromImage(bmp))
            {
                DrawAxes(g, Size);
                if (Func != null)
                {
                    PointF p = Map(Func(0)), q;
                    for (float t = TMin + dT; t <= TMax; t += dT)
                    {
                        q = Map(Func(t));
                        g.DrawLine(new Pen(ForeColor), p, q);
                        p = q;
                    }
                    g.DrawLine(new Pen(ForeColor), p, Map(Func(TMax)));
                }
            }
            e.Graphics.DrawImageUnscaled(bmp, Point.Empty);            
        }

        private void DrawAxes(Graphics g,Size sz)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            float dx,dy;
            if (MinPoint.X <= 0 && MaxPoint.X > 0)
            {
                dx = -MinPoint.X / (MaxPoint.X - MinPoint.X) * sz.Width;
                g.DrawLine(new Pen(HorizAxisColor), dx, 0, dx, sz.Height);
            }
            else if (MinPoint.X > 0) dx = 0;
            else dx = sz.Width;
            if (MinPoint.Y <= 0 && MaxPoint.Y > 0)
            {
                dy = -MinPoint.Y / (MaxPoint.Y - MinPoint.Y) * sz.Height;
                g.DrawLine(new Pen(VertAxisColor), 0, dy, sz.Width, dy);
            }
            else if (MinPoint.Y > 0) dy = 0;
            else dy = sz.Height;

            float y = MinPoint.Y,fy=(MaxPoint.Y-MinPoint.Y)/10;
            for(int j=0;j<10;j++)
            {
                if (y + j * fy == 0) continue;
                var Y = (y-MinPoint.Y + j * fy) / (MaxPoint.Y - MinPoint.Y) * sz.Height;
                g.DrawLine(new Pen(HorizAxisColor),dx-5,Y,dx+5,Y);
                g.DrawString(""+Math.Round(y+j*fy,2), Font, new SolidBrush(HorizAxisColor), dx + 5, Y + 5);
            }

            float x = MinPoint.Y, fx = (MaxPoint.X - MinPoint.X) / 10;
            for (int j = 0; j < 10; j++)
            {
                if (x + j * fx==0) continue;
                var X = (x - MinPoint.X + j * fx) / (MaxPoint.X - MinPoint.X) * sz.Width;
                g.DrawLine(new Pen(VertAxisColor), X, dy - 5, X, dy + 5);
                g.DrawString("" + Math.Round(x + j * fx,2), Font, new SolidBrush(VertAxisColor), X + 5, dy + 5);
            }
        }

        private PointF Map(PointF p)
        {
            float x = p.X-MinPoint.X, y = p.Y-MinPoint.Y;
            var w = MaxPoint.X - MinPoint.X;
            var h = MaxPoint.Y - MinPoint.Y;
            x = x * (Width) / w;
            y = y * (Height) / h;            
            return new PointF(x, y);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
