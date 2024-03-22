using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace R2D2.Commons.Types
{
    public class Polar
    {
        public double T, P;
        public Polar(double t=0,double p=0)
        {
            T = t;
            P = p;
        }

        public Point3D ToPoint3D() => new Point3D(Math.Cos(P) * Math.Cos(T), Math.Sin(P), Math.Cos(P) * Math.Sin(T));
    }
}
