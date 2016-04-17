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
using System.Linq;

namespace Transformation_lab2.ViewModel
{
    public class TransformationViewModel : INotifyPropertyChanged
    {
        private Matrix<double> Q;
        private Point AroundPoint = new Point();

        #region Constants
        private const string MoveCheckBox = "MoveCheckBox";
        private const string RotateCheckBox = "RotateCheckBox";
        private const string ScaleCheckBox = "ScaleCheckBox";
        #endregion

        #region Variables
        private string Methodname = string.Empty;
        private double DrawCanvasWidtdh;
        private double DrawCanvasHeight;
        private bool canExecute = true;
        public bool LastPointInsert { get; set; }
        int number = 1;
        int pointName = 1;
        #endregion

        #region Constructor
        public TransformationViewModel()
        {
            pointCollection.CollectionChanged += PointCollection_CollectionChanged;
            ComboboxChecked = new RelayCommand(TransformationType, param => this.canExecute);
            RunTransformation = new RelayCommand(RunTransformClick, param => this.canExecute);
            ClearCanvas = new RelayCommand(ClearCollections, param => this.canExecute);
        }
        #endregion

        #region MainMatrix
        private MathNet.Numerics.LinearAlgebra.Matrix<double> GetRotateMatrix()
        {
            var angleD = (double.Parse(Angle) * (System.Math.PI / 180.0)) * -1;
            var oldX = (AroundPoint.X * (1 - Math.Cos(angleD))) + (AroundPoint.Y * Math.Sin(angleD));
            var oldY = (AroundPoint.Y * (1 - Math.Cos(angleD))) - (AroundPoint.X * Math.Sin(angleD));

            Matrix<double> R =
                DenseMatrix.OfArray(new double[,] {
                                                    { Math.Cos(angleD), Math.Sin(angleD), 0},
                                                    {-Math.Sin(angleD), Math.Cos(angleD), 0},
                                                    {oldX, oldY, 1},
                                                  });
            return R;
        }

        private MathNet.Numerics.LinearAlgebra.Matrix<double> GetScaleMatrix()
        {
            var scaleX = double.Parse(ScaleX);
            var scaleY = double.Parse(ScaleY);
            Matrix<double> S = DenseMatrix.OfArray(new double[,] {
                                                    {scaleX,0,0},
                                                    {0,scaleY,0},
                                                    { AroundPoint.X * (1-scaleX), AroundPoint.Y * (1-scaleY),1},
                                                 });
            return S;
        }

        private MathNet.Numerics.LinearAlgebra.Matrix<double> GeMoveMatrix()
        {
            var moveX = double.Parse(MoveX);
            var moveY = double.Parse(MoveY);
            return DenseMatrix.OfArray(new double[,] {
                                                    { 1, 0, 0},
                                                    { 0, 1, 0},
                                                    { moveX, moveY, 1},
                                                  });
        }
        #endregion

        #region Methods
        private Vector<double> Vector(Point point)
        {
            return Vector<double>.Build.DenseOfArray(new double[] { point.X, point.Y, 1 });
        }

        private Point PointOutVector(Vector<double> vector)
        {
            Point p = new Point();
            p.X = vector[0];
            p.Y = vector[1];
            return p;
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

        #endregion        

        #region DrawControls
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
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
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
            DrawCanvasWidtdh = 1000;
            DrawCanvasHeight = 1000;
            double thickness = 5;
            double WidthCenterCanvas = DrawCanvasWidtdh / 2;
            double WidthCenterHeight = DrawCanvasHeight / 2;
            SolidColorBrush yellowBrush = new SolidColorBrush();
            yellowBrush.Color = Colors.Orange;
            Label l = new Label();
            l.Background = yellowBrush;
            l.FontWeight = FontWeights.Bold;
            p.X = (int)p.X;
            p.Y = (int)p.Y;
            if (p.X < WidthCenterCanvas && p.Y > WidthCenterHeight)
            {
                l.Margin = new Thickness(p.X + thickness, p.Y, 0, 0);
                l.Content = string.Format("{0} - ({1},{2})", pointName, (p.X - WidthCenterCanvas).ToString(), "-" + (p.Y - WidthCenterHeight).ToString());

            }
            if (p.X < WidthCenterCanvas && p.Y < WidthCenterHeight)
            {
                l.Margin = new Thickness(p.X + thickness, p.Y + thickness, 0, 0);
                l.Content = string.Format("{0} - ({1},{2})", pointName, (p.X - WidthCenterCanvas).ToString(), ((WidthCenterHeight) - p.Y).ToString());

            }
            if (p.X > WidthCenterCanvas && p.Y < WidthCenterHeight)
            {
                l.Margin = new Thickness(p.X + thickness, p.Y, 0, 0);
                l.Content = string.Format("{0} - ({1},{2})", pointName, (p.X - (WidthCenterCanvas)).ToString(), ((WidthCenterHeight) - p.Y).ToString());

            }
            if (p.X > WidthCenterCanvas && p.Y > WidthCenterHeight)
            {
                l.Margin = new Thickness(p.X + thickness, p.Y, 0, 0);
                l.Content = string.Format("{0} - ({1},{2})", pointName, (p.X - (WidthCenterCanvas)).ToString(), ((WidthCenterHeight) - p.Y).ToString());

            }
            pointName++;
            return l;
        }

        private void DrawLine(ObservableCollection<Point> points)
        {
            var nx = points[0];
            foreach (var item in points)
            {
                NewChildrenCanvas.Add(DrawLine(nx.X, nx.Y, item.X, item.Y));
                nx.X = item.X;
                nx.Y = item.Y;
            }
        }
        #endregion

        #region Run Transform Methods
        /// <summary>
        /// Bigins the movement 
        /// </summary>
        public void RunMove()
        {
            var pointAfterTransf = new ObservableCollection<Point>();
            NewPointCollection = PointCollection;

            AroundPoint = PointCollection[SelectedIndexPointsComboBox];
            var MoveMatrix = GeMoveMatrix();

            foreach (var item in PointCollection)
            {
                var newP = PointOutVector(Vector(item) * MoveMatrix);
                pointAfterTransf.Add(newP);
            }
            DrawLine(pointAfterTransf);
             NewPointCollection = pointAfterTransf;
        }

        /// <summary>
        /// Begins the rotations
        /// </summary>
        public void RunRotate()
        {
            var pointAfterTransf = new ObservableCollection<Point>();
            var pointAround = SelectedIndexPointsComboBox;
            NewPointCollection = PointCollection;
            int x = 0;
            AroundPoint = NewPointCollection[SelectedIndexPointsComboBox];
            var RotateMatrix = GetRotateMatrix();

            foreach (var item in NewPointCollection)
            {
                if (x == pointAround)
                {
                    x++;
                    pointAfterTransf.Add(item);
                    continue;
                }
                var newP = PointOutVector(Vector(item) * RotateMatrix);
                NewChildrenCanvas.Add(CreateEllipse(newP.X, newP.Y));
                NewChildrenCanvas.Add((CreateLabel(newP)));
                pointAfterTransf.Add(newP);
            }
            DrawLine(pointAfterTransf);
            NewPointCollection = pointAfterTransf;
        }


        public ObservableCollection<Point> RunRotate(ObservableCollection<Point> points, int index, double angle)
        {
            Angle = angle.ToString();
            var pointAfterTransf = new ObservableCollection<Point>();
            var pointAround = SelectedIndexPointsComboBox;
            NewPointCollection = points;
            int x = 0;
            AroundPoint = NewPointCollection[index];
            var RotateMatrix = GetRotateMatrix();

            foreach (var item in NewPointCollection)
            {
                if (x == pointAround)
                {
                    x++;
                    pointAfterTransf.Add(item);
                    continue;
                }
                var newP = PointOutVector(Vector(item) * RotateMatrix);
                pointAfterTransf.Add(newP);
            }
            DrawLine(pointAfterTransf);
            return pointAfterTransf;
        }

        /// <summary>
        /// Bigins the scales
        /// </summary>
        public void RunScale()
        {
            var pointAfterTransf = new ObservableCollection<Point>();
            int x = 0;
            NewPointCollection = PointCollection;
            int index = SelectedIndexPointsComboBox;
            Point X = new Point();

            AroundPoint = NewPointCollection[index];
            var ScaleMatrix = GetScaleMatrix();

            foreach (var item in NewPointCollection)
            {
               
                var newP = PointOutVector(Vector(item) * ScaleMatrix);
                if (x == index)
                {
                    pointAfterTransf.Add(item);
                    X.X = item.X;
                    X.Y = item.Y;
                    x++;
                    continue;
                }
                if (x == NewPointCollection.Count - 1)
                {
                    var p = pointAfterTransf[0];
                    pointAfterTransf.Add(p);
                    continue;
                }
                //NewChildrenCanvas.Add(DrawLine(X.X, X.Y, newP.X, newP.Y));
                //NewChildrenCanvas.Add(CreateEllipse(newP.X, newP.Y));
                //NewChildrenCanvas.Add(CreateLabel(newP));
                pointAfterTransf.Add(newP);
                x++;
            }
            DrawLine(pointAfterTransf);
            NewPointCollection = pointAfterTransf;
        }

        public ObservableCollection<Point> RunScale(ObservableCollection<Point> points, int index2, double scaleX, double scaleY)
        {
            

            var pointAfterTransf = new ObservableCollection<Point>();
            int x = 0;
            NewPointCollection = points;
            int index = index2;
            Point X = new Point();

            AroundPoint = NewPointCollection[index];


            Matrix<double> S = DenseMatrix.OfArray(new double[,] {
                                                    {scaleX,0,0},
                                                    {0,scaleY,0},
                                                    { AroundPoint.X * (1-scaleX), AroundPoint.Y * (1-scaleY),1},
                                                 });
            var ScaleMatrix = S;

            foreach (var item in NewPointCollection)
            {

                var newP = PointOutVector(Vector(item) * ScaleMatrix);
                if (x == index)
                {
                    pointAfterTransf.Add(item);
                    X.X = item.X;
                    X.Y = item.Y;
                    x++;
                    continue;
                }
                if (x == NewPointCollection.Count - 1)
                {
                    var p = pointAfterTransf[0];
                    pointAfterTransf.Add(p);
                    continue;
                }
                pointAfterTransf.Add(newP);
                x++;
            }
            return pointAfterTransf;
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
            NewPointCollection.Clear();
        }
        #endregion

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

        private string scaleX = "1,5";
        public string ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        private string scaleY = "1,5";
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

        private ObservableCollection<Point> newPointCollection = new ObservableCollection<Point>();
        public ObservableCollection<Point> NewPointCollection = new ObservableCollection<Point>();


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

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }
        #endregion

        private Matrix<double> Mainmatrix(Point around, double x = 0, double y = 0,
        double sx = 0, double sy = 0,
        double angleD = 0)
        {
            Matrix<double> M =
                DenseMatrix.OfArray(new double[,] {
                                                    { 1, 0, 0},
                                                    { 0, 1, 0},
                                                    { x, y, 1},
                                                   });
            Matrix<double> S =
                DenseMatrix.OfArray(new double[,] {
                                                    {sx,0,0},
                                                    {0,sy,0},
                                                    {around.X * (1-sx), around.Y * (1-sy),1},
                                                  });
            Matrix<double> R =
                DenseMatrix.OfArray(new double[,] {
                                                    { Math.Cos(angleD), Math.Sin(angleD), 0},
                                                    {-Math.Sin(angleD), Math.Cos(angleD), 0},
                                                    {               0,               0, 1},
                                                  });

            Q = M * S * R;
            return Q;
        }
    }
}
