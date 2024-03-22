using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace R2D2.Commons.Shapes
{
    public class Sphere
    {
        public Sphere(Point3D center,double radius,int num_phi,int num_theta)
        {
            Center = center;
            Radius = radius;
            NPhi = num_phi;
            NTheta = num_theta;

        }

        public Point3D Center;
        public double Radius;
        public int NPhi, NTheta;

        public void AddTo(MeshGeometry3D mesh)
        {
            double phi0, theta0;
            double dphi = Math.PI / NPhi;
            double dtheta = 2 * Math.PI / NTheta;

            phi0 = 0;
            double y0 = Radius * Math.Cos(phi0);
            double r0 = Radius * Math.Sin(phi0);
            for (int i = 0; i < NPhi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = Radius * Math.Cos(phi1);
                double r1 = Radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    Center.X + r0 * Math.Cos(theta0),
                    Center.Y + y0,
                    Center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    Center.X + r1 * Math.Cos(theta0),
                    Center.Y + y1,
                    Center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < NTheta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        Center.X + r0 * Math.Cos(theta1),
                        Center.Y + y0,
                        Center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        Center.X + r1 * Math.Cos(theta1),
                        Center.Y + y1,
                        Center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddSmoothTriangle(mesh, dict, pt00, pt11, pt10);
                    AddSmoothTriangle(mesh, dict, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;                
            }
        }

        public MeshGeometry3D ToMesh()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            AddTo(mesh);           
            return mesh;
        }
        Dictionary<Point3D, int> dict = new Dictionary<Point3D, int>();

        private void AddSmoothTriangle(MeshGeometry3D mesh, Dictionary<Point3D, int> dict, Point3D point1, Point3D point2, Point3D point3)
        {
            int index1, index2, index3;

            // Find or create the points.
            if (dict.ContainsKey(point1)) index1 = dict[point1];
            else
            {
                index1 = mesh.Positions.Count;
                mesh.Positions.Add(point1);
                dict.Add(point1, index1);
            }

            if (dict.ContainsKey(point2)) index2 = dict[point2];
            else
            {
                index2 = mesh.Positions.Count;
                mesh.Positions.Add(point2);
                dict.Add(point2, index2);
            }

            if (dict.ContainsKey(point3)) index3 = dict[point3];
            else
            {
                index3 = mesh.Positions.Count;
                mesh.Positions.Add(point3);
                dict.Add(point3, index3);
            }

            // If two or more of the points are
            // the same, it's not a triangle.
            if ((index1 == index2) ||
                (index2 == index3) ||
                (index3 == index1)) return;

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }
    }
}
