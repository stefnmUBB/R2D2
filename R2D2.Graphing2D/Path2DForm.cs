using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R2D2.Graphing2D
{
    public partial class Path2DForm : Form
    {
        public Path2DForm()
        {
            InitializeComponent();
            Canvas.BackColor = Color.Snow;
            Canvas.ForeColor = Color.Black;           
        }

        public Func<float, PointF> Func
        { get => Canvas.Func; set { Canvas.Func = value; Canvas.Invalidate(); } }
        public PointF MinPoint
        { get => Canvas.MinPoint; set => Canvas.MinPoint = value; }
        public PointF MaxPoint
        { get => Canvas.MaxPoint; set => Canvas.MaxPoint = value; }
        public float TMin { get => Canvas.TMin; set => Canvas.TMin = value; }
        public float TMax { get => Canvas.TMax; set => Canvas.TMax = value; }
        public float dT { get => Canvas.dT; set => Canvas.dT = value; }
        public string CaptionText { get => Canvas.CaptionText; set => Canvas.CaptionText = value; }

        public Color HorizAxisColor { get => Canvas.HorizAxisColor; set => Canvas.HorizAxisColor = value; } 
        public Color VertAxisColor { get => Canvas.VertAxisColor; set => Canvas.VertAxisColor = value; } 

        private void Path2DForm_Resize(object sender, EventArgs e)
        {
            Canvas.Invalidate();
        }
    }
}
