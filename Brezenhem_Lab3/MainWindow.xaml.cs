using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Brezenhem_Lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Transformation_lab2.ViewModel.TransformationViewModel transform = new Transformation_lab2.ViewModel.TransformationViewModel();

        private const int CenterXY = 500;
        private int SizePixel = 1;
        private bool KeyDownBool = false;
        private Point startPoint = new Point();

        ObservableCollection<Point> pointCollection = new ObservableCollection<Point>();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;


            //SolidColorBrush blueBrush = new SolidColorBrush();
            //blueBrush.Color = Colors.Blue;
            //SolidColorBrush blackBrush = new SolidColorBrush();
            //blackBrush.Color = Colors.Black;
            //Ellipse ellipse = new Ellipse { Width = 200, Height = 200 };
            //ellipse.Margin = new Thickness(CenterXY+ SizePixel, CenterXY-(200- SizePixel), 0, 0);
            //SolidColorBrush redBrush = new SolidColorBrush();
            //redBrush.Color = Colors.Red;
            //ellipse.Fill = redBrush;
            //ellipse.StrokeThickness = 1;
            //MainCanvas.Children.Add(ellipse);

            // Brezenhemalgorithm(CenterXY, CenterXY, 200);
        }



        private void Brezenhemalgorithm(double startX, double startY, int radius, int widthPixel = 1)
        {
            double BUFFER = startX - startY;

            int x = 0;
            int y = radius;
            int delta = 3 - 2 * (radius);

            while (x <= y)
            {
                if (delta >= 0)
                {

                    y -= widthPixel;
                    x += widthPixel;
                    delta = delta + 4 * (x - y) + 10;
                    CreateARectangle(widthPixel, widthPixel, x + startX, y + startY, ChildrenCanvas);
                    pointCollection.Add(new Point { X = x + startX, Y = y + startY });
                    continue;
                }
                if (delta < 0)
                {
                    x += widthPixel;
                    delta = delta + (4 * x) + 6;
                    CreateARectangle(widthPixel, widthPixel, x + startX, y + startY, ChildrenCanvas);
                    pointCollection.Add(new Point { X = x + startX, Y = y + startY });
                    continue;
                }
            }


            Matrix<double> M =
                DenseMatrix.OfArray(new double[,] {
                                                    { 0, 1},
                                                    { 1, 0},
                                                   });


            var newPointCollection = new ObservableCollection<Point>();
            foreach (var item in pointCollection)
            {
                var p = Vector<double>.Build.DenseOfArray(new double[] { item.X, item.Y });
                var newP = M * p;
                double newX = Math.Abs(newP[0]);
                double newY = Math.Abs(newP[1]);
                CreateARectangle(widthPixel, widthPixel, newX + BUFFER, newY - BUFFER, ChildrenCanvas);
                Point pp = new Point();
                pp.X = newX + BUFFER;
                pp.Y = newY - BUFFER;
                newPointCollection.Add(pp);
            }

            foreach (var item in newPointCollection)
            {
                pointCollection.Add(item);
            }

            Matrix<double> M2 =
               DenseMatrix.OfArray(new double[,] {
                                                    { 1, 0},
                                                    { 0, -1},
                                                  });
            var rotateColection = transform.RunRotate(pointCollection, 0, -90);
            foreach (var item in rotateColection)
            {
                var p = Vector<double>.Build.DenseOfArray(new double[] { item.X, item.Y });
                var newP = M2 * p;
                double newX = Math.Abs(newP[0]);
                double newY = Math.Abs(newP[1]);
                CreateARectangle(widthPixel, widthPixel, newX - radius, newY - radius + widthPixel, ChildrenCanvas);
                Point pp = new Point();
                pp.X = newX - radius;
                pp.Y = newY - radius + widthPixel;
                pointCollection.Add(pp);
            }


            var scaleCollection = transform.RunScale(pointCollection, 0, 1, -1);
            foreach (var item in scaleCollection)
            {
                CreateARectangle(widthPixel, widthPixel, item.X, item.Y - radius * 2 + widthPixel, ChildrenCanvas);
            }

        }





        public void CreateARectangle(int widthRectangle, int heigthRectangle, double x, double y, Canvas canvas)
        {
            Rectangle blueRectangle = new Rectangle();
            blueRectangle.Height = widthRectangle;
            blueRectangle.Width = heigthRectangle;

            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            blueRectangle.Fill = blueBrush;

            Canvas.SetLeft(blueRectangle, x - widthRectangle / 2);
            Canvas.SetTop(blueRectangle, y - heigthRectangle / 2);

            canvas.Children.Add(blueRectangle);
        }

        private void CreateEllipse(double desiredCenterX, double desiredCenterY)
        {
            double width = 5; double height = 5;
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            ellipse.Fill = redBrush;
            ellipse.StrokeThickness = 4;
            ChildrenCanvas.Children.Add(ellipse);



        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            const double margin = 10;
            double xmin = MainCanvas.ActualWidth / 2;
            double xmax = MainCanvas.ActualWidth;
            double ymin = margin;
            double ymax = MainCanvas.ActualHeight / 2;

            var AxisM = xmin;
            var AxisM2 = 0.0;
            const double step = 100;
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(MainCanvas.ActualWidth, ymax)));
            for (double x = step; x <= MainCanvas.ActualWidth - step; x += step)
            {
                //MainCanvas.Children.Add(CreateLabel(x-10, ymax - margin / 2));
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
                if (x < xmin)
                {
                    AxisM = Math.Abs(AxisM) - step;
                    AxisM = AxisM * -1;
                    MainCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, AxisM.ToString(), 0, 10));
                }
                else if (x == xmin)
                {
                    MainCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, "0", 0, 10));
                }
                else
                {
                    AxisM2 = AxisM2 + step;
                    MainCanvas.Children.Add(CreateLabel(x, ymax + margin / 2, AxisM2.ToString(), 0, 10));
                }

            }

            var AxisMY = ymax;
            var AxisMY2 = 0.0;
            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = redBrush;
            xaxis_path.Data = xaxis_geom;

            MainCanvas.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, MainCanvas.ActualHeight)));
            for (double y = step; y <= MainCanvas.ActualHeight - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));

                if (y < ymax)
                {
                    AxisMY = AxisMY - step;
                    MainCanvas.Children.Add(CreateLabel(xmin - margin / 2, y, AxisMY.ToString(), 10, 0));
                }
                else if (y == xmin)
                {

                }
                else
                {
                    AxisMY2 = Math.Abs(AxisMY2) + step;
                    AxisMY2 = AxisMY2 * -1;
                    MainCanvas.Children.Add(CreateLabel(xmin - margin / 2, y, AxisMY2.ToString(), 10, 0));
                }


            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = redBrush;
            yaxis_path.Data = yaxis_geom;

            MainCanvas.Children.Add(yaxis_path);
        }

        private Label CreateLabel(double x, double y, string content, double thX, double thY)
        {
            Label l = new Label();
            l.Content = content;
            l.Margin = new Thickness(x + thX, y + thY, 0, 0);
            l.FontWeight = FontWeights.Bold;
            return l;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !KeyDownBool)
            {
                var position = e.GetPosition(this);
                startPoint.X = position.X;
                startPoint.Y = position.Y;
                DrawButton.IsEnabled = true;
            }
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            Brezenhemalgorithm(startPoint.X, startPoint.Y, int.Parse(RadiusTextBox.Text), int.Parse(PixelSize.Text));
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ChildrenCanvas.Children.Clear();
            KeyDownBool = false;
            pointCollection = new ObservableCollection<Point>();
            DrawButton.IsEnabled = false;
        }
    }
}
