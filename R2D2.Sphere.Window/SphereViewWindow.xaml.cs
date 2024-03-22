using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using R2D2.Graphing2D;
using R2D2.Commons.Shapes;
using R2D2.Commons.Extensions;
using R2D2.Commons.Types;

namespace R2D2.ProgramSphere
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SphereViewWindow : System.Windows.Window
    {       
        public SphereViewWindow()
        {
            InitializeComponent();                             
        }

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Camera = new PerspectiveCamera
            {
                FieldOfView = 60
            };
            MainViewport.Camera = Camera;
            PositionCamera();
            DefineModel(MainModel3Dgroup);

            ModelVisual3D model_visual = new ModelVisual3D
            {
                Content = MainModel3Dgroup
            };

            // Display the main visual to the viewport.
            MainViewport.Children.Add(model_visual);
        }        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {            
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    CameraR -= CameraDR;
                    if (CameraR < 3.5) CameraR = 3.5;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    CameraR += CameraDR;
                    if (CameraR > 10) CameraR = 10;
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        private bool Viewport_msdown = false;
        private System.Drawing.Point Viewport_mspos;
        private void MainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Viewport_msdown = true;
            Viewport_mspos = System.Windows.Forms.Control.MousePosition;
        }

        private void MainViewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Viewport_msdown = false;
        }

        private void MainViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Viewport_msdown) return;
            var pt = System.Windows.Forms.Control.MousePosition;
            var dx = pt.X - Viewport_mspos.X;
            var dy = pt.Y - Viewport_mspos.Y;
            CameraPhi += Math.Sign(dy) * 0.03;
            CameraTheta += Math.Sign(dx) * 0.03;
            if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
            else if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
            PositionCamera();
            Viewport_mspos = pt;

        }

        private void MainViewport_MouseLeave(object sender, MouseEventArgs e)
        {
            Viewport_msdown = false;
        }

        private void MainViewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            CameraR += Math.Sign(e.Delta) * 0.03;
            if (CameraR < 3.5) CameraR = 3.5;
            if (CameraR > 10) CameraR = 10;
        }

        #endregion

        #region Input Events
        private void Th0_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            t0 = th0.Value * 2 * Math.PI / th0.Maximum;
            DefineModel(MainModel3Dgroup);
        }

        private void Ph0_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            p0 = ph0.Value * 2 * Math.PI / ph0.Maximum;
            DefineModel(MainModel3Dgroup);
        }

        private void Th1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            t1 = th1.Value * 2 * Math.PI / th1.Maximum;
            DefineModel(MainModel3Dgroup);
        }

        private void Ph1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            p1 = ph1.Value * 2 * Math.PI / ph1.Maximum;
            DefineModel(MainModel3Dgroup);
        }

        private void ChBSphere_Checked(object sender, RoutedEventArgs e)
        {
            draw_sphere = true;
            DefineModel(MainModel3Dgroup);
        }

        private void ChBSphere_Unchecked(object sender, RoutedEventArgs e)
        {
            draw_sphere = false;
            DefineModel(MainModel3Dgroup);
        }

        private void AddRotationControl(RotationInfo ri)
        {
            var DockPanel = new DockPanel() { Tag = ri };
            var Label = new Label() { MinWidth = 30 };
            Label.Content = ri.Axis;
            var ctm = new ContextMenu();
            var rm = new MenuItem
            {
                Header = "Remove",
                Tag = DockPanel
            };
            rm.Click += RotationSlider_Remove;
            ctm.Items.Add(rm);
            DockPanel.ContextMenu = ctm;
            DockPanel.SetDock(Label, Dock.Left);
            var Slider = new Slider()
            {
                Maximum = 100,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
                TickFrequency = 5,
                Focusable = false,
                IsSnapToTickEnabled = true,
                Tag = ri
            };
            Slider.ValueChanged += RotationSlider_ValueChanged;
            DockPanel.Children.Add(Label);
            DockPanel.Children.Add(Slider);
            Rotations.Add(ri);
            RotationsList.Children.Add(DockPanel);
        }

        private void RotationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var el = sender as Slider;
            var ri = el.Tag as RotationInfo;
            ri.Value = el.Value * 2 * Math.PI / el.Maximum;
            DefineModel(MainModel3Dgroup);
        }

        private void RotationSlider_Remove(object sender, EventArgs e)
        {
            var el = sender as MenuItem;
            var dp = el.Tag as DockPanel;
            var ri = dp.Tag as RotationInfo;
            Rotations.Remove(ri);
            RotationsList.Children.Remove(dp);
            DefineModel(MainModel3Dgroup);
        }

        private void RXAddAction_Click(object sender, RoutedEventArgs e) => AddRotationControl(new RotationInfo('X', 0));       

        private void RYAddAction_Click(object sender, RoutedEventArgs e) => AddRotationControl(new RotationInfo('Y', 0));        

        private void RZAddAction_Click(object sender, RoutedEventArgs e) => AddRotationControl(new RotationInfo('Z', 0));

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Button button = sender as Button;
                ContextMenu contextMenu = button.ContextMenu;
                contextMenu.PlacementTarget = button;
                contextMenu.IsOpen = true;
            }
            e.Handled = true;
        }
        
        #endregion

        #region Menu

        private void Menu_File_New_Click(object sender, RoutedEventArgs e)
        {
            t0 = p0 = t1 = p1 = 0;
            draw_sphere = true;
            Rotations = new List<RotationInfo>();
            RotationsList.Children.Clear();
            DefineModel(MainModel3Dgroup);
        }

        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog
            {
                Filter = "R2D2 files | *.r2d2"
            };
            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fn = sd.FileName;
                using (var w = new BinaryWriter(File.OpenWrite(fn)))
                {
                    w.Write("_sphere");
                    w.Write(t0); w.Write(p0);
                    w.Write(t1); w.Write(p1);
                    w.Write(draw_sphere);
                    w.Write(Rotations.Count);
                    for (int i = 0, sz = Rotations.Count; i < sz; i++)
                    {
                        var el = Rotations[i];
                        w.Write(el.Axis);
                        w.Write(el.Value);
                    }
                    w.Close();
                }
            }
        }

        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog od = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "R2D2 files | *.r2d2"
            };
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fn = od.FileName;
                try
                {
                    using (var r = new BinaryReader(File.OpenRead(fn)))
                    {
                        var str = r.ReadString();
                        if (str != "_sphere")
                        {
                            MessageBox.Show("The model contained in file is not a sphere.\nPlease try another R2D2 program.");
                            return;
                        }
                        t0 = r.ReadDouble(); p0 = r.ReadDouble();
                        t1 = r.ReadDouble(); p1 = r.ReadDouble();
                        draw_sphere = r.ReadBoolean();
                        var n = r.Read();
                        Rotations.Clear();
                        for (int i = 0; i < n; i++)
                        {
                            Rotations.Add(new RotationInfo(r.ReadChar(), r.ReadDouble()));
                        }
                        r.Close();
                    }
                    DefineModel(MainModel3Dgroup);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error occured...");

                }
            }
        }

        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void Menu_Tools_Graph_XY_Click(object sender, RoutedEventArgs e)
        {
            if (XYGraphForm == null)
            {
                DefineXYGraphForm();
                XYGraphForm.Func = t => new System.Drawing.PointF((float)Trajectory(t).X, (float)Trajectory(t).Y);
            }
            if (XYGraphForm.Visible) XYGraphForm.Hide(); else XYGraphForm.Show();
            Menu_Tools_Graph_XY.IsChecked = XYGraphForm.Visible;
        }

        private void Menu_Tools_Graph_YZ_Click(object sender, RoutedEventArgs e)
        {
            if (YZGraphForm == null)
            {
                DefineYZGraphForm();
                YZGraphForm.Func = t => new System.Drawing.PointF((float)Trajectory(t).Z, (float)Trajectory(t).Y);
            }
            if (YZGraphForm.Visible) YZGraphForm.Hide(); else YZGraphForm.Show();
            Menu_Tools_Graph_YZ.IsChecked = YZGraphForm.Visible;
        }

        private void Menu_Tools_Graph_XZ_Click(object sender, RoutedEventArgs e)
        {
            if (XZGraphForm == null)
            {
                DefineXZGraphForm();
                XZGraphForm.Func = t => new System.Drawing.PointF((float)Trajectory(t).X, (float)Trajectory(t).Z);
            }
            if (XZGraphForm.Visible) XZGraphForm.Hide(); else XZGraphForm.Show();
            Menu_Tools_Graph_XZ.IsChecked = XZGraphForm.Visible;
        }

        private void Menu_Tools_Graph_TP_Click(object sender, RoutedEventArgs e)
        {
            if (TPGraphForm == null)
            {
                DefineTPGraphForm();
                TPGraphForm.Func = t => new System.Drawing.PointF((float)Trajectory(t).ToPolar().T, (float)Trajectory(t).ToPolar().P);
            }
            if (TPGraphForm.Visible) TPGraphForm.Hide(); else TPGraphForm.Show();
            Menu_Tools_Graph_TP.IsChecked = TPGraphForm.Visible;
        }
        #endregion

        #region Graphs

        #region SetOwner(Form form,Window owner) Helper

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        public static void SetOwner(System.Windows.Forms.Form form, System.Windows.Window owner)
        {
            WindowInteropHelper helper = new WindowInteropHelper(owner);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());
        }

        #endregion

        Path2DForm XYGraphForm, XZGraphForm, YZGraphForm, TPGraphForm;
        private void DefineXYGraphForm()
        {
            XYGraphForm = new Path2DForm();
            SetOwner(XYGraphForm, this);
            XYGraphForm.ClientSize = new System.Drawing.Size(300, 300);
            XYGraphForm.Text = "Graph (X,Y)";
            XYGraphForm.dT = 0.01f;
            XYGraphForm.MinPoint = new System.Drawing.PointF(-1.2f, -1.2f);
            XYGraphForm.MaxPoint = new System.Drawing.PointF(1.2f, 1.2f);
            XYGraphForm.VertAxisColor = System.Drawing.Color.FromArgb(0, 0, 255);
            XYGraphForm.HorizAxisColor = System.Drawing.Color.FromArgb(255, 0, 0);
            XYGraphForm.FormClosing += (o, ev) => { ev.Cancel = true; Menu_Tools_Graph_XY_Click(null, null); };
        }

        private void DefineXZGraphForm()
        {
            XZGraphForm = new Path2DForm();
            SetOwner(XZGraphForm, this);
            XZGraphForm.ClientSize = new System.Drawing.Size(300, 300);
            XZGraphForm.Text = "Graph (X,Z)";
            XZGraphForm.dT = 0.01f;
            XZGraphForm.MinPoint = new System.Drawing.PointF(-1.2f, -1.2f);
            XZGraphForm.MaxPoint = new System.Drawing.PointF(1.2f, 1.2f);
            XZGraphForm.VertAxisColor = System.Drawing.Color.FromArgb(0, 0, 255);
            XZGraphForm.HorizAxisColor = System.Drawing.Color.FromArgb(0, 255, 0);
            XZGraphForm.FormClosing += (o, ev) => { ev.Cancel = true; Menu_Tools_Graph_XZ_Click(null, null); };
        }

        private void DefineYZGraphForm()
        {
            YZGraphForm = new Path2DForm();
            SetOwner(YZGraphForm, this);
            YZGraphForm.ClientSize = new System.Drawing.Size(300, 300);
            YZGraphForm.Text = "Graph (Z,Y)";
            YZGraphForm.dT = 0.01f;
            YZGraphForm.MinPoint = new System.Drawing.PointF(-1.2f, -1.2f);
            YZGraphForm.MaxPoint = new System.Drawing.PointF(1.2f, 1.2f);
            YZGraphForm.VertAxisColor = System.Drawing.Color.FromArgb(0, 255, 0);
            YZGraphForm.HorizAxisColor = System.Drawing.Color.FromArgb(255, 0, 0);
            YZGraphForm.FormClosing += (o, ev) => { ev.Cancel = true; Menu_Tools_Graph_YZ_Click(null, null); };
        }

        private void DefineTPGraphForm()
        {
            TPGraphForm = new Path2DForm();
            SetOwner(TPGraphForm, this);
            TPGraphForm.ClientSize = new System.Drawing.Size(300, 300);
            TPGraphForm.Text = "Graph (θ,φ)";
            TPGraphForm.dT = 0.01f;
            TPGraphForm.MinPoint = new System.Drawing.PointF(-3.5f, -3.5f);
            TPGraphForm.MaxPoint = new System.Drawing.PointF(3.5f, 3.5f);
            TPGraphForm.VertAxisColor = System.Drawing.Color.FromArgb(0, 255, 255);
            TPGraphForm.HorizAxisColor = System.Drawing.Color.FromArgb(255, 0, 255);
            TPGraphForm.FormClosing += (o, ev) => { ev.Cancel = true; Menu_Tools_Graph_TP_Click(null, null); };
        }

        private void TryLoadGraph(Path2DForm form,Func<float,System.Drawing.PointF> func)        
        {
            try
            {
                form.Func = func;
            }
            catch (Exception) { }
        }
        #endregion

        #region 3D Mechanics                   
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera.
        private PerspectiveCamera Camera;

        // The camera's current location.
        private double CameraPhi = Math.PI / 6.0;       
        private double CameraTheta = Math.PI / 6.0;     
        private double CameraR = 3.5;

        private const double CameraDPhi = 0.1;
        private const double CameraDTheta = 0.1;
        private const double CameraDR = 0.1;       
      
        private void PositionCamera()
        {
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            Camera.Position = new Point3D(x, y, z);
            Camera.LookDirection = new Vector3D(-x, -y, -z);
            Camera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }

        #endregion


        #region Simulation Logic       

        public double t0 = 0, p0 = 0;
        public double t1 = 0, p1 = 0;
        bool draw_sphere = true;
        private List<RotationInfo> Rotations = new List<RotationInfo>();
        Func<double, Point3D> Trajectory;
        private double alpha = 0;

        double gamma0 = 0, gamma1 = 0;

        void GetAlpha()
        {
            var pt = new Polar(t1,p1).ToPoint3D().RotY(-t0).RotZ(-p0);
            double x = pt.X, y = pt.Y, z = pt.Z;           
            alpha = -Math.Atan2(y, z);            
        }        

        void GetGammas()
        {
            var pt0 = new Polar(t0, p0).ToPoint3D().RotY(-t0).RotZ(-p0).RotX(-alpha);
            var pt1 = new Polar(t1, p1).ToPoint3D().RotY(-t0).RotZ(-p0).RotX(-alpha);

            double x = pt0.X, z = pt0.Z;          
            gamma0 = Math.Atan2(z,x);                                 
            x = pt1.X; z = pt1.Z;
            gamma1 = Math.Atan2(z, x);         
        }

        private double DeltaGamma
        {
            get
            {
                double lambda = gamma1 - gamma0, pi = Math.PI;
                var f = Math.Sign(lambda) * Math.Sign(pi - Math.Abs(lambda)) * Math.Min(Math.Abs(lambda), 2 * Math.PI - Math.Abs(lambda));
                return (f == 0) ? (p0 == p1 && t0 == t1) ? 0 : Math.PI : f;
            }
        }

        private void DefineModel(Model3DGroup model_group)
        {
            #region Clear the scene
            MainModel3Dgroup.Children.Clear();
            DefineLights();
            #endregion

            GetAlpha();
            GetGammas();

            DiffuseMaterial surface_material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(120, 255, 128, 0)));

            #region Draw Axes
            model_group.Children.Add(new GeometryModel3D(
                new Segment(new Point3D(-1.2, 0, 0), new Point3D(1.2, 0, 0), new Vector3D(0,0.1,0)).ToMesh(true),
                new DiffuseMaterial(Brushes.Blue))
                );

            model_group.Children.Add(new GeometryModel3D(
               new Segment(new Point3D(0, 0, -1.2), new Point3D(0, 0, 1.2), new Vector3D(0.1,0.1,0.1)).ToMesh(true),
               new DiffuseMaterial(Brushes.Green))
               );            

            model_group.Children.Add(new GeometryModel3D(
               new Segment(new Point3D(0, -1.2, 0), new Point3D(0, 1.2, 0), new Vector3D(0.1,0.1,0.1)).ToMesh(true),
               new DiffuseMaterial(Brushes.Red))
               );
            #endregion

            #region Draw Sphere
            if (draw_sphere)
            {
                model_group.Children.Add(new GeometryModel3D(new Sphere(new Point3D(0, 0, 0), 1, 50, 50).ToMesh(), surface_material)
                { BackMaterial = surface_material });
            }
            #endregion

            #region Draw First Position      
            var computed_position_0 = new Polar(t0, p0).ToPoint3D().Rot(Rotations);
            model_group.Children.Add(new GeometryModel3D(
              new Segment(computed_position_0.Scale(0.9), new Point3D(0,0,0), new Vector3D(0, 0.1, 0)).ToMesh(true),
              new DiffuseMaterial(Brushes.Black))
              );
            model_group.Children.Add(new GeometryModel3D(new Sphere(computed_position_0, 0.05, 5, 5).ToMesh(), new DiffuseMaterial(Brushes.Fuchsia)));
            #endregion

            #region Draw Last Position

            var computed_position_1 = new Polar(t1, p1).ToPoint3D().Rot(Rotations);
            model_group.Children.Add(new GeometryModel3D(
             new Segment(computed_position_1.Scale(0.9), new Point3D(0, 0, 0), new Vector3D(0, 0.1, 0)).ToMesh(true),
             new DiffuseMaterial(Brushes.Black))
             );
            model_group.Children.Add(new GeometryModel3D(new Sphere(computed_position_1, 0.05, 5, 5).ToMesh(), new DiffuseMaterial(Brushes.Gainsboro)));
            #endregion

            #region Compute And Draw Trajectory
            Func<double, double> theta = t => gamma0 +t*DeltaGamma;
            Func<double, double> phi = t => 0;            
            Trajectory = t => new Point3D(Math.Cos(phi(t)) * Math.Cos(theta(t)), Math.Sin(phi(t)), Math.Cos(phi(t)) * Math.Sin(theta(t))).RotX(alpha).RotZ(p0).RotY(t0).Rot(Rotations);
            model_group.Children.Add(new GeometryModel3D(
                new Trajectory(Trajectory, 0,1).ToMesh(),
                new DiffuseMaterial(new SolidColorBrush(Colors.Black))));
            #endregion

            #region Compute Graphs
            TryLoadGraph(XYGraphForm, t => new System.Drawing.PointF((float)Trajectory(t).X, (float)Trajectory(t).Y));
            TryLoadGraph(XZGraphForm, t => new System.Drawing.PointF((float)Trajectory(t).X, (float)Trajectory(t).Z));
            TryLoadGraph(YZGraphForm, t => new System.Drawing.PointF((float)Trajectory(t).Y, (float)Trajectory(t).Z));
            TryLoadGraph(TPGraphForm, t => new System.Drawing.PointF((float)Trajectory(t).ToPolar().T, (float)Trajectory(t).ToPolar().P));
            #endregion
        }

        #endregion
    }
}
