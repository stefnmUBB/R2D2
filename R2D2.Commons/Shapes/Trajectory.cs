using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace R2D2.Commons.Shapes
{
    public class Trajectory
    {
        public Trajectory(Func<double,Point3D> func,double tmin,double tmax)
        {
            F = func;
            TMin = tmin;
            TMax = tmax;
        }

        public MeshGeometry3D ToMesh()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double dt = (TMax-TMin)/50;
                             
            for (double t = TMin; t <= TMax; t += dt) 
            {
                var p =F(t);               
                new Sphere(p, 0.01, 3, 3).AddTo(mesh);
            }

            return mesh;
        }

        public Func<double, Point3D> F;
        public double TMin;
        public double TMax;
    }
}
