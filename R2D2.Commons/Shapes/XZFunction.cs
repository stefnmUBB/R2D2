using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace R2D2.Commons.Shapes
{
    public class XZFunction
    {
        public XZFunction(Func<double,double,double> func,double xmin,double zmin,double xmax,double zmax)
        {
            F = func;
            XMin = xmin; XMax = xmax;
            ZMin = zmin; ZMax = zmax;
            texture_xscale = XMax - XMin;
            texture_zscale = ZMax - ZMin;
        }

        public Func<double, double, double> F;
        public double XMin, XMax;
        public double ZMin, ZMax;
        private double texture_xscale;
        private double texture_zscale;

        public MeshGeometry3D ToMesh()
        {
            PointDictionary.Clear();
            MeshGeometry3D mesh = new MeshGeometry3D();
            double dx = 0.01;
            double dz = 0.01;
            for (double x = XMin; x <= XMax; x += dx)
            {
                for (double z = ZMin; z <= ZMax; z += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    try
                    {
                        Point3D p00 = new Point3D(x, F(x, z), z);
                        Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                        Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                        Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                        AddTriangle(mesh, p00, p01, p11);
                        AddTriangle(mesh, p00, p11, p10);
                    }
                    catch(Exception) { }
                }               
            }                     
            return mesh;
        }

        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {           
            int index1 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point1);
            int index2 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point2);
            int index3 = AddPoint(mesh.Positions, mesh.TextureCoordinates, point3);          
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        private Dictionary<Point3D, int> PointDictionary = new Dictionary<Point3D, int>();       
        private int AddPoint(Point3DCollection points, PointCollection texture_coords, Point3D point)
        {          
            if (PointDictionary.ContainsKey(point))
                return PointDictionary[point];            
            points.Add(point);
            PointDictionary.Add(point, points.Count - 1);            
            texture_coords.Add(
                new Point(
                    (point.X - XMin) * texture_xscale,
                    (point.Z - ZMin) * texture_zscale));            
            return points.Count - 1;
        }
    }
}
