using System;
using System.Collections.Generic;
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

namespace ShutOffLines
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point sp = new Point();
        Point sp2 = new Point();

        #region Variables
        bool leftButton = true;

        byte[] FirstBinarPoint = new byte[4];
        byte[] SecontBinarPoint = new byte[4];

        static int RectangleWidth = 400;
        static int RectangleHeigth = 300;

        //min x
        static int StartXRectangle = 100;
        //max y
        static int StartYRectangle = 100;
        //max x
        static int EndXRectangle = RectangleWidth + StartXRectangle;
        //min y
        static int EndYRectangle = RectangleHeigth + StartYRectangle;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            MainDrawCanvas.Children.Add(DrawRectangle());
        }

        private void ShutOffLine(byte[] array, ref Point startPoint, ref Point endPoint)
        {
            var index = GetIndex(array);

            if (index == 0) // слева
            {
                startPoint.Y += (endPoint.Y - startPoint.Y) * (StartXRectangle - startPoint.X) / (endPoint.X - startPoint.X);
                startPoint.X = StartXRectangle;
            }
            else if (index == 1) // справа
            {
                startPoint.Y += ((endPoint.Y - startPoint.Y) * (EndXRectangle - startPoint.X)) / (endPoint.X - startPoint.X);
                startPoint.X = EndXRectangle;
            }
            else if (index == 2) //снизу
            {
                startPoint.X += (endPoint.X - startPoint.X) * (EndYRectangle - startPoint.Y) / (endPoint.Y - startPoint.Y);
                startPoint.Y = EndYRectangle;
            }
            else if (index == 3) //сверху
            {
                startPoint.X += (endPoint.X - startPoint.X) * (StartYRectangle - startPoint.Y) / (endPoint.Y - startPoint.Y);
                startPoint.Y = StartYRectangle;
            }
        }

        private int GetIndex(byte[] array)
        {
            int x = 0;
            foreach (var item in array)
            {
                if (item == 1)
                {
                    return x;
                }
                x++;
            }
            return x;
        }

        private int GetPointBinary(double x, double y, byte[] array)
        {
            array[0] = (byte)(x >= StartXRectangle ? 0 : 1);
            array[1] = (byte)(x <= EndXRectangle ? 0 : 1);
            array[2] = (byte)(y >= EndYRectangle ? 1 : 0);
            array[3] = (byte)(y <= StartYRectangle ? 1 : 0);
            return BitConverter.ToInt32(array, 0);
        }

        private Line DrawLine(double x1, double y1, double x2, double y2, double StrokeThickness = 2, int color = 0)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.StrokeThickness = StrokeThickness;
            SolidColorBrush solid = new SolidColorBrush();
            solid.Color = color == 0 ? Colors.Green : Colors.Blue;
            line.Stroke = solid;
            return line;
        }

        private Rectangle DrawRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = RectangleWidth;
            rectangle.Height = RectangleHeigth;
            Canvas.SetLeft(rectangle, StartXRectangle);
            Canvas.SetTop(rectangle, StartYRectangle);
            SolidColorBrush solid = new SolidColorBrush();
            solid.Color = Colors.Red;
            rectangle.Stroke = solid;
            return rectangle;
        }

        private void MainDrawCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            #region LeftButton
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (leftButton)
                {
                    var position = e.GetPosition(this);
                    sp.X = position.X;
                    sp.Y = position.Y;
                    leftButton = false;
                }
                else
                {
                    var position = e.GetPosition(this);
                    sp2.X = position.X;
                    sp2.Y = position.Y;
                    MainDrawCanvas.Children.Add(DrawLine(sp.X, sp.Y, sp2.X, sp2.Y, 2));
                }
            }
            #endregion

            #region StartShutOff
            if (e.RightButton == MouseButtonState.Pressed)
            {
                StartAlgo();
                StartAlgo();
            }
            #endregion
        }

        private void StartAlgo()
        {
            int triv = 0;
            int nowtriv = 0;
            var one = GetPointBinary(sp.X, sp.Y, FirstBinarPoint);
            var two = GetPointBinary(sp2.X, sp2.Y, SecontBinarPoint);

            //тривиальная видимость = 0
            triv = one | two;
            //тривиальная невидимость !=0
            nowtriv = one & two;

            if (triv == 0)
            {
                MainDrawCanvas.Children.Add(DrawLine(sp.X, sp.Y, sp2.X, sp2.Y, 1, 1));
            }
            if (nowtriv != 0)
            {
                MessageBox.Show("Вне границы");
                return;
            }
            else
            {
                ShutOffLine(FirstBinarPoint, ref sp, ref sp2);
                ShutOffLine(SecontBinarPoint, ref sp2, ref sp);

                var one2 = GetPointBinary(sp.X, sp.Y, FirstBinarPoint);
                var two2 = GetPointBinary(sp2.X, sp2.Y, SecontBinarPoint);

                MainDrawCanvas.Children.Clear();
                MainDrawCanvas.Children.Add(DrawRectangle());
                MainDrawCanvas.Children.Add(DrawLine(sp.X, sp.Y, sp2.X, sp2.Y, 1, 1));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sp = new Point();
            sp2 = new Point();
            MainDrawCanvas.Children.Clear();
            MainDrawCanvas.Children.Add(DrawRectangle());
            leftButton = true;
        }
    }
}
