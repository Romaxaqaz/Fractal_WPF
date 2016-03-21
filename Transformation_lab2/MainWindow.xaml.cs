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
            double xmin = DrawCanvas.ActualWidth / 2 + 20;
            double xmax = DrawCanvas.ActualWidth;
            double ymin = margin;
            double ymax = DrawCanvas.ActualHeight / 2;
            const double step = 20;
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(DrawCanvas.ActualWidth, ymax)));
            for (double x = step; x <= DrawCanvas.ActualWidth - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

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
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = redBrush;
            yaxis_path.Data = yaxis_geom;

            DrawCanvas.Children.Add(yaxis_path);

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
                }
            }
        }

    }
}
