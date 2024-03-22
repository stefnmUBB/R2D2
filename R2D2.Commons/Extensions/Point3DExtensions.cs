using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using R2D2.Commons.Types;

namespace R2D2.Commons.Extensions
{
    public static class Point3DExtensions
    {
        public static Point3D RotX(this Point3D v, double a)
        {
            return new Point3D(v.X, v.Y * Math.Cos(a) - v.Z * Math.Sin(a), v.Z * Math.Cos(a) + v.Y * Math.Sin(a));
        }
        public static Point3D RotY(this Point3D v, double a)
        {
            return new Point3D(v.X * Math.Cos(a) - v.Z * Math.Sin(a), v.Y, v.Z * Math.Cos(a) + v.X * Math.Sin(a));
        }

        public static Point3D RotZ(this Point3D v, double a)
        {
            return new Point3D(v.X * Math.Cos(a) - v.Y * Math.Sin(a), v.Y * Math.Cos(a) + v.X * Math.Sin(a), v.Z);
        }

        public static Point3D Rot(this Point3D v, RotationInfo ri)
        {
            switch (char.ToUpper(ri.Axis))
            {
                case 'X': return v.RotX(ri.Value);
                case 'Y': return v.RotY(ri.Value);
                case 'Z': return v.RotZ(ri.Value);
                default: return v;
            }
        }

        public static Point3D Rot(this Point3D v, List<RotationInfo> lri)
        {
            Point3D result = v;
            for (int i = 0, cnt = lri.Count; i < cnt; i++)
                result = result.Rot(lri[i]);
            return result;
        }

        public static Point3D Scale(this Point3D p,double s)
        {
            return new Point3D(p.X * s, p.Y * s, p.Z * s);
        }

        public static Polar ToPolar(this Point3D p)
        {
            return new Polar(
                Math.Acos(p.X / Math.Sqrt(p.X * p.X + p.Y * p.Y)) * (p.Y < 0 ? -1 : 1),
                Math.Asin(p.Z / Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z))
                );
        }
    }
}
