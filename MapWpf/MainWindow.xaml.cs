﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point scrollMousePoint = new Point();
        double hOff = 1;
        double vOff = 1;
        private int HeightMin = 750;
        private int HeightMax = 8033;
        private int WidthMax = 14705;
        private int WidthMin = 1388;
        private double zoom = 1.1;
        private TopGraph top;
        private Graph g;
        private double thickness = 10.0;
        Point gridRootPoint = new Point();

        public TopGraph TopInGraph
        {
            get
            {
                if (top == null)
                {
                    top = new TopGraph();
                }
                return top;
            }
            set => top = value;
        }
        public Graph GetGraph
        {
            get
            {
                if (g == null)
                {
                    g = new Graph();
                }
                return g;
            }
            set => g = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            GetData();
            Loading();
        }

        void GetData()
        {

            GetGraph.TopGraphs = TopInGraph.ConvertTextToList(@"C:\Users\Admin\source\repos\MapWpf\MapWpf\Data\Data.txt");
            GetGraph.NumberOfTop = GetGraph.TopGraphs.Count;
            var tops = GetGraph.Disjkstra(5, 100);
            for (int i = 0; i < tops.Item2.Length; i++)
            {
                if (i == tops.Item2.Length - 1)
                {
                    return;
                }
                var line = DrawLine(GetGraph.TopGraphs[tops.Item2[i]].GetPoints.X, GetGraph.TopGraphs[tops.Item2[i + 1]].GetPoints.X,
                    GetGraph.TopGraphs[tops.Item2[i]].GetPoints.Y, GetGraph.TopGraphs[tops.Item2[i + 1]].GetPoints.Y, thickness);
                GridRoot.Children.Add(line);
            }
            ScrollRoot.ScrollToVerticalOffset(vOff+ GetGraph.TopGraphs[tops.Item2[0]].GetPoints.X);
            ScrollRoot.ScrollToHorizontalOffset(hOff+ GetGraph.TopGraphs[tops.Item2[0]].GetPoints.Y);

        }



        void Loading()
        {
            GridRoot.Height = HeightMax;
            GridRoot.Width = WidthMax;
            string path = @"C:\Users\Admin\source\repos\MapWpf\MapWpf\Data\ImageData.jpg";
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            bitmap.Freeze();

            ImageRoot.Source = bitmap;
            GCCLoad();
        }

        void GCCLoad()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            TopGraph top = new TopGraph();

            Graph graph = new Graph
            {
                TopGraphs = top.ConvertTextToList(@"C:\Users\Admin\source\repos\MapWpf\MapWpf\Data\Data.txt")
            };
            graph.NumberOfTop = graph.TopGraphs.Count;
            var result = graph.Disjkstra(5, 100);
            int[] tops = result.Item2;
            for (int i = 0; i < tops.Length; i++)
            {
                //graph.TopGraphs[i];
            }
        }

        private void ScrollRoot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollRoot.ReleaseMouseCapture();
        }

        private void ScrollRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(ScrollRoot);
            hOff = ScrollRoot.HorizontalOffset;
            vOff = ScrollRoot.VerticalOffset;
            //this.Cursor = Cursors.Hand;
            ScrollRoot.CaptureMouse();
        }

        private void ScrollRoot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollRoot.IsMouseCaptured)
            {
                ScrollRoot.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollRoot).Y));
                ScrollRoot.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollRoot).X));
            }
        }


        private double _zoomValue = 1.0;
        private void ScrollRoot_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var position = e.MouseDevice.GetPosition(GridRoot);
            Debug.WriteLine("V :"+ ScrollRoot.VerticalOffset);
            Debug.WriteLine("H :" + ScrollRoot.HorizontalOffset);
            if (e.Delta > 0)
            {
                if (GridRoot.Height <= HeightMax || GridRoot.Width <= WidthMax)
                {
                    var gs= GridRoot.RenderTransform;
                    GridRoot.Height = GridRoot.Height * zoom;
                    GridRoot.Width = GridRoot.Width * zoom;
                    ReLayoutLine(GridRoot, zoom, (double)thickness +2);
                    //ScrollRoot.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollRoot).Y));
                    //ScrollRoot.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollRoot).X));
                }
                
                //position.X = position.X * 1.1;
                //position.Y = position.Y * 1.1;

                //position.X = position.X * 2;
                //position.Y = position.Y * 2;
                //ScrollRoot.ScrollToVerticalOffset(ScrollRoot.VerticalOffset*(1+1.1));
                //ScrollRoot.ScrollToHorizontalOffset(ScrollRoot.HorizontalOffset*(1+1.1));
            }
            else
            {
                if (GridRoot.Height >= HeightMin || GridRoot.Width >= WidthMin)
                {

                    GridRoot.Height = GridRoot.Height / zoom;
                    GridRoot.Width = GridRoot.Width / zoom;
                    ReLayoutLine(GridRoot, Math.Pow(zoom, -1),(double)thickness/10);
                    //position.X = position.X / 1.1;
                    //position.Y = position.Y / 1.1;
                }
               
            }
            
            
            Debug.WriteLine("AV :" + ScrollRoot.VerticalOffset);
            Debug.WriteLine("AH :" + ScrollRoot.HorizontalOffset +"\n");
            GCCLoad();
            //this.Cursor=new Cursor(Cursor.)
            /*
            var position = e.MouseDevice.GetPosition(ImageRoot);
            e.Handled = true;
            var renderTransformValue = ImageRoot.RenderTransform.Value;
            if (e.Delta > 0)
                renderTransformValue.ScaleAtPrepend(1.1, 1.1, position.X, position.Y);
            else
                renderTransformValue.ScaleAtPrepend(1 / 1.1, 1 / 1.1, position.X, position.Y);

            ImageRoot.RenderTransform = new MatrixTransform(renderTransformValue);
            GCCLoad();*/
        }

        private void ReLayoutLine(Grid grid, double zoom,double thickness)
        {
            var lines = grid.Children.OfType<Line>().ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].X1 = lines[i].X1 * zoom;
                lines[i].X2 = lines[i].X2 * zoom;
                lines[i].Y1 = lines[i].Y1 * zoom;
                lines[i].Y2 = lines[i].Y2 * zoom;
                lines[i].StrokeThickness = thickness;
            }

        }

        /// <summary>
        /// Return a circle 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Ellipse DrawMark(double radius, double x, double y)
        {
            var ellipse = new Ellipse
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(x, y, 0, 0),
                Fill = new SolidColorBrush(Colors.Red),
                Stroke = new SolidColorBrush(Colors.Black),
                Width = radius,
                Height = radius
            };
            return ellipse;
        }

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        private Line DrawLine(double x1, double x2, double y1, double y2, double thickness)
        {
            var line = new Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = thickness,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.Red)
            };
            return line;
        }

        private void GridRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scale.ScaleX = e.NewSize.Width / ImageRoot.Source.Width;
            scale.ScaleY = e.NewSize.Height / ImageRoot.Source.Height;
            Debug.WriteLine("sV :" + ScrollRoot.VerticalOffset);
            Debug.WriteLine("sH :" + ScrollRoot.HorizontalOffset);
        }
    }
}
