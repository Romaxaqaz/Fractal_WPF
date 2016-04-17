using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Transformation_lab2.ViewModel;

namespace Transformation_lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TransformationViewModel viewModel = new TransformationViewModel();

        private bool endPointBool = false;


        public MainWindow()
        {
            InitializeComponent();
            // Polyline.Points = new PointCollection(pointsCollection);
            Loaded += MainWindow_Loaded;
            SizeChanged += MainWindow_SizeChanged;
            this.DataContext = viewModel;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            const double margin = 10;
            double xmin = DrawCanvas.ActualWidth / 2;
            double xmax = DrawCanvas.ActualWidth;
            double ymin = margin;
            double ymax = DrawCanvas.ActualHeight / 2;

            var AxisM = xmin;
            var AxisM2 = 0.0;
            const double step = 100;
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(DrawCanvas.ActualWidth, ymax)));
            for (double x = step; x <= DrawCanvas.ActualWidth - step; x += step)
            {
                //DrawCanvas.Children.Add(CreateLabel(x-10, ymax - margin / 2));
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
                if(x< xmin)
                {
                    AxisM = Math.Abs(AxisM) - step;
                    AxisM = AxisM * -1;
                    DrawCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, AxisM.ToString(),0,10));
                }
                else if(x==xmin)
                {
                    DrawCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, "0", 0, 10));
                }
                else
                {
                    AxisM2 = AxisM2 + step;
                    DrawCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, AxisM2.ToString(), 0, 10));
                }

            }

            var AxisMY = ymax;
            var AxisMY2 = 0.0;
            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = redBrush;
            xaxis_path.Data = xaxis_geom;

            DrawCanvas.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, DrawCanvas.ActualHeight)));
            for (double y = step; y <= DrawCanvas.ActualHeight - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));

                if (y < ymax)
                {
                    AxisMY = AxisMY - step;
                    DrawCanvas.Children.Add(CreateLabel(xmin - margin / 2, y, AxisMY.ToString(),10, 0));
                }
                else if (y == xmin)
                {
                   
                }
                else
                {
                    AxisMY2 = Math.Abs(AxisMY2) + step;
                    AxisMY2 = AxisMY2 * -1;
                    DrawCanvas.Children.Add(CreateLabel(xmin - margin / 2, y, AxisMY2.ToString(), 10, 0));
                }


            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = redBrush;
            yaxis_path.Data = yaxis_geom;

            DrawCanvas.Children.Add(yaxis_path);

        }

        private Label CreateLabel(double x, double y,string content, double thX, double thY)
        {
            Label l = new Label();
            l.Content = content;
            l.Margin = new Thickness(x + thX, y+ thY, 0, 0);
            l.FontWeight = FontWeights.Bold;
            return l;
        }

        private void DrawCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!endPointBool)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var position = e.GetPosition(this);
                    viewModel.PointCollection.Add(position);
                }
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    viewModel.LastPointInsert = true;
                    var col = viewModel.PointCollection;
                    viewModel.PointCollection.Add(col[0]);
                    PolyLine.Points = new PointCollection(viewModel.PointCollection);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //if(MoveCheckBox.IsChecked==true)
            //{
            //    var l = CreatePolyline();
            //    Canvas.SetLeft(l, double.Parse(MoveXTextBox.Text));
            //    Canvas.SetTop(l, double.Parse(MoveYTextBox.Text));
            //    PolCanvas2.Children.Add(l);
            //}

            //if (RotateCheckBox.IsChecked == true)
            //{
            //    var l = CreatePolyline();
            //    RotateTransform rotateTransform1 = new RotateTransform(double.Parse(AngleTextBox.Text), viewModel.PointCollection[PointNumber.SelectedIndex].X, viewModel.PointCollection[PointNumber.SelectedIndex].Y);
            //    l.RenderTransform = rotateTransform1;
            //    PolCanvas2.Children.Add(l);
 
            //}

            //if (ScaleCheckBox.IsChecked == true)
            //{
            //    var l = CreatePolyline();
            //    ScaleTransform scale = new ScaleTransform(double.Parse(ScaleSXTextBox.Text), double.Parse(ScaleSYTextBox.Text), viewModel.PointCollection[PointNumber.SelectedIndex].X, viewModel.PointCollection[PointNumber.SelectedIndex].Y);
            //    l.RenderTransform = scale;
            //    PolCanvas2.Children.Add(l);
            //}

        }
        private Polyline CreatePolyline()
        {
            Polyline l = new Polyline();
            SolidColorBrush yellowBrush = new SolidColorBrush();
            yellowBrush.Color = Colors.Blue;
            l.Points = PolyLine.Points;
            l.StrokeThickness = 3;
            l.Stroke = yellowBrush;
            return l;
        }

        private void ScaleSXTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private Ellipse CreateEllipse(double desiredCenterX, double desiredCenterY)
        {
            double width = 20; double height = 20;
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            ellipse.Fill = redBrush;
            ellipse.StrokeThickness = 4;
            return ellipse;



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PolCanvas2.Children.Clear();
        }
    }
}
