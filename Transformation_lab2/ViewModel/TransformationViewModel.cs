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
using System.ComponentModel;
using Transformation_lab2.Command;

namespace Transformation_lab2.ViewModel
{
    public class TransformationViewModel : INotifyPropertyChanged
    {
        #region Constants
        private const string MoveCheckBox = "MoveCheckBox";
        private const string RotateCheckBox = "RotateCheckBox";
        private const string ScaleCheckBox = "ScaleCheckBox";
        #endregion

        private string Methodname = string.Empty;
        private double DrawCanvasWidtdh;
        private double DrawCanvasHeight;

        private bool canExecute = true;
        public bool LastPointInsert { get; set; }
        int number = 1;

        public TransformationViewModel()
        {
            pointCollection.CollectionChanged += PointCollection_CollectionChanged;
            ComboboxChecked = new RelayCommand(TransformationType, param => this.canExecute);
            RunTransformation = new RelayCommand(RunTransformClick, param => this.canExecute);
            ClearCanvas = new RelayCommand(ClearCollections, param => this.canExecute);
        }


        private void PointCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && !LastPointInsert)
            {
                var length = PointCollection.Count - 1;
                ChildrenCanvas.Add(CreateEllipse(PointCollection[length].X, PointCollection[length].Y));
                ChildrenCanvas.Add(CreateLabel(PointCollection[length]));
                Numberspoints.Add(number.ToString());
                number++;
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && LastPointInsert)
            {
                int step = 0;
                Point x = new Point();
                x = PointCollection[0];
                foreach (var item in PointCollection)
                {
                    if (step == 0)
                    {
                        step++;
                        continue;
                    }

                    ChildrenCanvas.Add(DrawLine(x.X, x.Y, item.X, item.Y));
                    x.X = item.X;
                    x.Y = item.Y;
                }
                LastPointInsert = false;
            }
        }

        /// <summary>
        /// Moves the point
        /// </summary>
        /// <param name="x">move x axis</param>
        /// <param name="y">move y a xic</param>
        /// <param name="point">required point</param>
        /// <returns></returns>
        private Point MoveTransform(double x, double y, Point point)
        {
            Point newPoint = new Point();
            Vector<double> v = Vector<double>.Build.DenseOfArray(new double[] { point.X, point.Y, 1 });
            Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                                                                  { 1, 0, 0},
                                                                  { 0, 1, 0},
                                                                  { x, y, 1},
                                                                 });


            var resultVector = v * A;
            newPoint.X = Math.Abs(resultVector[0]);
            newPoint.Y = Math.Abs(resultVector[1]);
            return newPoint;
        }

        /// <summary>
        /// Scale position point
        /// </summary>
        /// <param name="sx">x scale factor</param>
        /// <param name="sy">y scale factor</param>
        /// <param name="point">required point</param>
        /// <returns></returns>
        private Point ScaleTranssform(double sx, double sy, Point point)
        {
            Point newPoint = new Point();
            Vector<double> v = Vector<double>.Build.DenseOfArray(new double[] { point.X, point.Y, 1 });
            Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                                                                  {sx,0,0},
                                                                  {0,sy,0},
                                                                  {0,0,1},
                                                                 });
            var resultVector = A * v;
            newPoint.X = resultVector[0];
            newPoint.Y = resultVector[1];
            return newPoint;
        }

        /// <summary>
        /// Rotates point
        /// </summary>
        /// <param name="point">required point</param>
        /// <param name="arraund">around point</param>
        /// <param name="angle">angle of rotation</param>
        /// <returns></returns>
        private Point RotateTranssform(Point point, Point arraund, double angle)
        {
            Point newPoint = new Point();

            Vector<double> v = Vector<double>.Build.DenseOfArray(new double[] { point.X - arraund.X, point.Y - arraund.Y, 1 });
            var angleD = (angle * (System.Math.PI / 180.0)) * -1;
            Matrix<double> A = DenseMatrix.OfArray(new double[,] {
                                                                  { Math.Cos(angleD), Math.Sin(angleD), 0},
                                                                  {-Math.Sin(angleD), Math.Cos(angleD), 0},
                                                                  {               0,               0, 1},
                                                                 });

            var res = A * v;
            newPoint.X = res[0] + arraund.X;
            newPoint.Y = res[1] + arraund.Y;
            return newPoint;
        }

        /// <summary>
        /// Creates a line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>Line</returns>
        private Line DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Green;

            // Set Line's width and color
            line.StrokeThickness = 4;
            line.Stroke = redBrush;
            return line;
        }

        /// <summary>
        /// Creates an ellipse
        /// </summary>
        /// <param name="desiredCenterX"></param>
        /// <param name="desiredCenterY"></param>
        /// <returns>Ellipse</returns>
        private Ellipse CreateEllipse(double desiredCenterX, double desiredCenterY)
        {
            double width = 20; double height = 20;
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            Label l = new Label();
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            l.Margin = new Thickness(left, top, 0, 0);
            l.Content = number.ToString();
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;
            ellipse.Fill = redBrush;
            ellipse.StrokeThickness = 4;
            return ellipse;



        }

        /// <summary>
        /// Create a lable with the specified coordinates
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Label CreateLabel(Point p)
        {
            DrawCanvasWidtdh = 800;
            DrawCanvasHeight = 600;
            Label l = new Label();
            p.X = (int)p.X;
            p.Y = (int)p.Y;
            if (p.X < DrawCanvasWidtdh / 2 && p.Y > DrawCanvasHeight / 2)
            {
                l.Margin = new Thickness(p.X + 10, p.Y + 10, 0, 0);
                l.Content = string.Format("{0},{1}", (p.X - DrawCanvasWidtdh / 2).ToString(), "-" + (p.Y - DrawCanvasHeight / 2).ToString());

            }
            if (p.X < DrawCanvasWidtdh / 2 && p.Y < DrawCanvasHeight / 2)
            {
                l.Margin = new Thickness(p.X + 10, p.Y + 10, 0, 0);
                l.Content = string.Format("{0},{1}", (p.X - DrawCanvasWidtdh / 2).ToString(), ((DrawCanvasHeight / 2) - p.Y).ToString());

            }
            if (p.X > DrawCanvasWidtdh / 2 && p.Y < DrawCanvasHeight / 2)
            {
                l.Margin = new Thickness(p.X + 10, p.Y + 10, 0, 0);
                l.Content = string.Format("{0},{1}", (p.X - (DrawCanvasWidtdh / 2)).ToString(), ((DrawCanvasHeight / 2) - p.Y).ToString());

            }
            if (p.X > DrawCanvasWidtdh / 2 && p.Y > DrawCanvasHeight / 2)
            {
                l.Margin = new Thickness(p.X + 10, p.Y + 10, 0, 0);
                l.Content = string.Format("{0},{1}", (p.X - (DrawCanvasWidtdh / 2)).ToString(), ((DrawCanvasHeight / 2) - p.Y).ToString());

            }
            return l;
        }

        /// <summary>
        /// Bigins the movement 
        /// </summary>
        public void RunMove()
        {
            double mX = double.Parse(MoveX);
            double mY = double.Parse(MoveY);
            Point x = new Point();
            x = PointCollection[1];
            x.X += mX;
            x.Y += mY;
            var pointAr = PointCollection;
            foreach (var item in PointCollection)
            {
                var newP = MoveTransform(mX, mY, item);
                NewChildrenCanvas.Add(CreateEllipse(newP.X, newP.Y));
                NewChildrenCanvas.Add(DrawLine(x.X, x.Y, newP.X, newP.Y));
                NewChildrenCanvas.Add(CreateLabel(newP));
                x.X = newP.X;
                x.Y = newP.Y;
            }
        }

        /// <summary>
        /// Begins the rotations
        /// </summary>
        public void RunRotate()
        {
            var pointAround = SelectedIndexPointsComboBox;
            int x = 0;
           
            foreach (var item in PointCollection)
            {
                if (x == pointAround)
                {
                    x++;
                    continue;
                }
                var newP = RotateTranssform(item, PointCollection[SelectedIndexPointsComboBox], double.Parse(Angle));
                NewChildrenCanvas.Add(CreateEllipse(newP.X, newP.Y));
                NewChildrenCanvas.Add((CreateLabel(newP)));
            }
        }

        /// <summary>
        /// Bigins the scales
        /// </summary>
        public void RunScale()
        {
            foreach (var item in PointCollection)
            {
                double SX = double.Parse(ScaleX);
                double SY = double.Parse(ScaleY);
                var newP = ScaleTranssform(SX, SY, item);
                NewChildrenCanvas.Add(CreateEllipse(newP.X, newP.Y));
                NewChildrenCanvas.Add(CreateLabel(newP));
            }
        }

        /// <summary>
        /// Get the type of transformation 
        /// </summary>
        /// <param name="obj">Name type</param>
        private void TransformationType(object obj)
        {
            Methodname = (string)obj;
        }

        /// <summary>
        /// Begin transformation
        /// </summary>
        /// <param name="obj"></param>
        private void RunTransformClick(object obj)
        {
            TransformationRun(Methodname);
        }

        private void TransformationRun(string name)
        {
            switch (name)
            {
                case MoveCheckBox:
                    RunMove();
                    break;
                case RotateCheckBox:
                    RunRotate();
                    break;
                case ScaleCheckBox:
                    RunScale();
                    break;
            }

        }

        private void ClearCollections(object obj)
        {
            NewChildrenCanvas.Clear();
        }

        #region Properties
        private int selectedIndexPointsComboBox;
        public int SelectedIndexPointsComboBox
        {
            get { return selectedIndexPointsComboBox; }
            set
            {
                selectedIndexPointsComboBox = value;
                OnPropertyChanged("SelectedIndexPointsComboBox");
            }
        }

        private string moveX = "0";
        public string MoveX
        {
            get { return moveX; }
            set { moveX = value; }
        }

        private string moveY = "0";
        public string MoveY
        {
            get { return moveY; }
            set { moveY = value; }
        }

        private string angle = "0";
        public string Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        private string scaleX = "0";
        public string ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        private string scaleY = "0";
        public string ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }
        #endregion

        #region Collections
        private ObservableCollection<object> childrenCanvas = new ObservableCollection<object>();
        public ObservableCollection<object> ChildrenCanvas
        {
            get { return childrenCanvas; }
            set
            {
                childrenCanvas = value;
                OnPropertyChanged("ChildrenCanvas");
            }
        }

        private ObservableCollection<object> newchildrenCanvas = new ObservableCollection<object>();
        public ObservableCollection<object> NewChildrenCanvas
        {
            get { return newchildrenCanvas; }
            set
            {
                newchildrenCanvas = value;
                OnPropertyChanged("NewChildrenCanvas");
            }
        }

        private ObservableCollection<Point> pointCollection = new ObservableCollection<Point>();
        public ObservableCollection<Point> PointCollection
        {
            get { return pointCollection; }
            set
            {
                pointCollection = value;
                OnPropertyChanged("PointCollection");
            }
        }

        private ObservableCollection<string> numberspoints = new ObservableCollection<string>();
        public ObservableCollection<string> Numberspoints
        {
            get { return numberspoints; }
            set
            {
                numberspoints = value;
                OnPropertyChanged("Numberspoints");
            }
        }
        #endregion

        #region Commands
        private ICommand comboboxChecked;
        public ICommand ComboboxChecked
        {
            get
            {
                return comboboxChecked;
            }
            set
            {
                comboboxChecked = value;
            }
        }

        private ICommand runTransformation;
        public ICommand RunTransformation
        {
            get
            {
                return runTransformation;
            }
            set
            {
                runTransformation = value;
            }
        }

        private ICommand clearCanvas;
        public ICommand ClearCanvas
        {
            get
            {
                return clearCanvas;
            }
            set
            {
                clearCanvas = value;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }

    }
}
