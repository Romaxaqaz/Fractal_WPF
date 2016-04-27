using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SeededAlgorithm
{
    public partial class MainWindow : Window
    {

        private static int PixelSize = 50;
        private static int HalfPixelSize = (PixelSize / 2) + 1;
        private DispatcherTimer timer = new DispatcherTimer();

        private ObservableCollection<Point> BorderPoint = new ObservableCollection<Point>();
        private ObservableCollection<Point> BlackPoint = new ObservableCollection<Point>();
        private Stack<Point> PointsStack = new Stack<Point>();

        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (PointsStack.Count != 0)
            {
                var startPoint = PointsStack.Pop();
                CreateARectangle(PixelSize, PixelSize, startPoint.X, startPoint.Y, RectangleColor.Black);
                BlackPoint.Add(startPoint);

                //rigth
                SearchNewButtonLR(startPoint, PixelSize);
                //top
                SearchNewButtonTB(startPoint, -PixelSize);
                //left
                SearchNewButtonLR(startPoint, -PixelSize);
                //buttom
                SearchNewButtonTB(startPoint, PixelSize);

            }
            else
            {
                timer.Stop();
                MessageBox.Show("Complete!");
            }
        }

        private void MainDrawCanvs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this);
                var newPoint = NewPointPosition(position);

                CreateARectangle(PixelSize, PixelSize, newPoint.X, newPoint.Y, RectangleColor.Red);
                BorderPoint.Add(newPoint);
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(this);
                var newPoint = NewPointPosition(position);

                CreateARectangle(PixelSize, PixelSize, newPoint.X, newPoint.Y, RectangleColor.Black);
                PointsStack.Push(newPoint);
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                timer.Start();
            }
        }

        private Point NewPointPosition(Point point)
        {
            Point newPoint = new Point();
            var x = (int)(point.X / PixelSize);
            var y = (int)(point.Y / PixelSize);

            newPoint.X = (x * PixelSize) + HalfPixelSize;
            newPoint.Y = (y * PixelSize) + HalfPixelSize;
            return newPoint;
        }

        public void CreateARectangle(int widthRectangle, int heigthRectangle, double x, double y, RectangleColor color)
        {
            Rectangle blueRectangle = new Rectangle();
            blueRectangle.Height = widthRectangle;
            blueRectangle.Width = heigthRectangle;

            SolidColorBrush blueBrush = new SolidColorBrush();
            SolidColorBrush blackBrush = new SolidColorBrush();
            if (color == RectangleColor.Black)
            {
                blueBrush.Color = Colors.Black;
            }
            else
            {
                blueBrush.Color = Colors.Red;
            }

            blackBrush.Color = Colors.Black;
            blueRectangle.Fill = blueBrush;
            blueRectangle.Stroke = blackBrush;

            Canvas.SetLeft(blueRectangle, x - widthRectangle / 2);
            Canvas.SetTop(blueRectangle, y - heigthRectangle / 2);

            MainDrawCanvs.Children.Add(blueRectangle);

        }

        private bool SearchNewButtonLR(Point point, int increment)
        {
            point.X += increment;
            var rightPoint = BorderPoint.FirstOrDefault(x => x == point);
            var blackPoint = BlackPoint.FirstOrDefault(x => x == point);
            if (rightPoint.X == 0 && blackPoint.X == 0)
            {
                PointsStack.Push(point);
                return true;
            }
            return false;
        }

        private bool SearchNewButtonTB(Point point, int increment)
        {
            point.Y += increment;
            var rightPoint = BorderPoint.FirstOrDefault(x => x == point);
            var blackPoint = BlackPoint.FirstOrDefault(x => x == point);
            if (rightPoint.Y == 0 && blackPoint.Y == 0)
            {
                PointsStack.Push(point);
                return true;
            }
            return false;
        }

        private void ClearData()
        {
            MainDrawCanvs.Children.Clear();
            PointsStack = new Stack<Point>();
            BorderPoint = new ObservableCollection<Point>();
            BlackPoint = new ObservableCollection<Point>();
        }

        private void StartAlgorithm()
        {
            if (BorderPoint.Count == 0)
            {
                MessageBox.Show("Задайте границу");
                return;
            }
            if (PointsStack.Count == 0)
            {
                MessageBox.Show("Задайте начальную точку");
                return;
            }
            try
            {
                var timeTick = int.Parse(TimeTick.Text);
                timer.Interval = new TimeSpan(0, 0, 0, 0, timeTick);
                timer.Start();
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите время шага");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var index = Convert.ToInt32(button.Tag);

            switch (index)
            {
                case 0:
                    StartAlgorithm();
                    break;
                case 1:
                    timer.Stop();
                    break;
                case 2:
                    ClearData();
                    break;
            }

        }
    }

    public enum RectangleColor
    {
        Black,
        Red
    }
}
