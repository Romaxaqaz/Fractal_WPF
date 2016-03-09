using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Fractal_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Random random = new Random((int)DateTime.Now.Ticks);
        private DispatcherTimer timer = new DispatcherTimer();

        private double sizeWith = 0.0;
        private double sizeHeight = 0.0;
        private bool TimerOnOff = false;

        private int CountStep = 0;
        private int ChildLines = 0;
        private int MaxAngle = 0;

        int count = 0;
        int stack = 0;

        public MainWindow()
        {
            InitializeComponent();
            CountStep = int.Parse(CountStepTextBox.Text);
            ChildLines = int.Parse(ChildLinesTextBox.Text);
            MaxAngle = int.Parse(AngleLinesTextBox.Text);
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += delegate
            {
                Create(sizeWith, 100, sizeWith, 10);
            };
        }

        private void Create(double stX, double stY, double nX, double nY, double StrokeThickness = 10)
        {
            StrokeThickness = 3;
            double x1 = stX;
            double y1 = stY;
            double x2 = nX;
            double y2 = nY;
            DrawFractalCanvas.Children.Add(DrawLine(x1, y1, x2, y2, StrokeThickness));
            int step = 0;
            while (step < CountStep)
            {
                count = random.Next(0, 100);

                double rX = random.NextDouble() * random.Next(10, MaxAngle);
                double rY = random.NextDouble() * random.Next(10, MaxAngle);

                var xCenter = (x1 + x2) / 2;
                var yCenter = (y1 + y2) / 2;

                if (count % 2 == 0)
                {
                    x1 += rX;
                    y1 += rY;
                }
                else
                {
                    x1 -= rX;
                    y1 += rY;
                }
                DrawFractalCanvas.Children.Add(DrawLine(x1, y1, xCenter, yCenter, StrokeThickness));
                if (stack < ChildLines && step == stack)
                {
                    stack++;
                    Create(x1, y1, x2, y2, 2);
                }
                x2 = xCenter;
                y2 = yCenter;
                StrokeThickness -= 0.15;
                step++;
            }
            stack = 0;
        }

        private Line DrawLine(double x1, double y1, double x2, double y2, double StrokeThickness)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.StrokeThickness = StrokeThickness;
            SolidColorBrush solid = new SolidColorBrush();
            solid.Color = Colors.White;
            line.Stroke = solid;
            return line;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if(TimerOnOff)
            {
                timer.Stop();
                TimerOnOff = false;
            }
            else
            {
                timer.Start();
                TimerOnOff = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            DrawFractalCanvas.Children.Clear();
        }

        private void DrawFractalCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            sizeWith = e.NewSize.Width / 3;
            sizeHeight = e.NewSize.Height / 3;

        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            Create(sizeWith, 100, sizeWith, 10);
        }
    }
}
