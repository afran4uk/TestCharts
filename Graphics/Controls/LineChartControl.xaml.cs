using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Charts.Utils;
using Point = System.Windows.Point;

namespace Charts.Controls
{
    /// <summary>
    /// Interaction logic for LineChartControl.xaml
    /// </summary>
    public partial class LineChartControl : UserControl
    {
        #region Fields

        private readonly double _percentMargin = 95;
        private readonly double _startXPos = 40;
        private readonly int _scaleCount = 10;

        private readonly DispatcherTimer _timer = new();

        private Chart _chart;
        private ChartResult _chartResult;

        private double _chartHeight;
        private double _chartWidth;
        private double _controlHeight;
        private double _controlWidth;
        
        private int _currentIndex;

        #endregion

        #region Dep Prop

        public bool IsMouseEnter
        {
            get => (bool)GetValue(IsMouseEnterProperty);
            set => SetValue(IsMouseEnterProperty, value);
        }

        public static readonly DependencyProperty IsMouseEnterProperty =
            DependencyProperty.Register(nameof(IsMouseEnter), typeof(bool), typeof(LineChartControl),
                new PropertyMetadata(false, null));

        public bool IsLoadData
        {
            get => (bool)GetValue(IsLoadDataProperty);
            set => SetValue(IsLoadDataProperty, value);
        }

        public static readonly DependencyProperty IsLoadDataProperty =
            DependencyProperty.Register(nameof(IsLoadData), typeof(bool), typeof(LineChartControl),
                new PropertyMetadata(false, null));

        public double SelectValue
        {
            get => (double)GetValue(SelectValueProperty);
            set => SetValue(SelectValueProperty, value);
        }

        public static readonly DependencyProperty SelectValueProperty =
            DependencyProperty.Register(nameof(SelectValue), typeof(double), typeof(LineChartControl),
                new PropertyMetadata(0.0, null));

        public double LastAddedValue
        {
            get => (double)GetValue(LastAddedValueProperty);
            set => SetValue(LastAddedValueProperty, value);
        }

        public static readonly DependencyProperty LastAddedValueProperty =
            DependencyProperty.Register(nameof(LastAddedValue), typeof(double), typeof(LineChartControl),
                new PropertyMetadata(0.0, null));

        public DateTime SelectValueDate
        {
            get => (DateTime)GetValue(SelectValueDateProperty);
            set => SetValue(SelectValueDateProperty, value);
        }

        public static readonly DependencyProperty SelectValueDateProperty =
            DependencyProperty.Register(nameof(SelectValueDate), typeof(DateTime), typeof(LineChartControl),
                new PropertyMetadata(new DateTime(), null));

        public IReadOnlyDictionary<int, (double Value, DateTime ValueDate)> ChartData
        {
            get => (IReadOnlyDictionary<int, (double Value, DateTime ValueDate)>)GetValue(ChartDataProperty);
            set => SetValue(ChartDataProperty, value);
        }

        public static readonly DependencyProperty ChartDataProperty =
            DependencyProperty.Register(nameof(ChartData), typeof(IReadOnlyDictionary<int, (double Value, DateTime ValueDate)>), typeof(LineChartControl),
                new PropertyMetadata(null, OnChartDataPropertyChanged));


        private static void OnChartDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineChartControl lineChartControl)
            {
                var dict = (IReadOnlyDictionary<int, (double Value, DateTime ValueDate)>)e.NewValue;
                if (dict.Any() == false)
                    return;

                lineChartControl.StartDrawChart(dict);
            }
        }

        private void StartDrawChart(IReadOnlyDictionary<int, (double Value, DateTime ValueDate)> dict)
        {
            ResetCharts();
            ChartData = dict;
            IsLoadData = true;
            _timer.Start();
        }

        #endregion

        public LineChartControl()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            ChartCanvas.MouseMove += OnMouseMove;
            ChartCanvas.MouseLeave += OnMouseLeave;
        }

        #region Events

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_chartResult == null)
                return;

            var currentPosition = e.GetPosition(ChartCanvas);

            var x = currentPosition.X;

            var xPos = (int)currentPosition.X / 1;

            if (x > _startXPos && x < _chartWidth)
            {
                _timer.Stop();
                IsMouseEnter = true;
            }
            else
                IsMouseEnter = false;

            if (IsMouseEnter == false)
            {
                RestartTimer();
                return;
            }

            if (IsMouseEnter)
            {
                var nearPoint = _chart.GetNearPoint(xPos);
                if (nearPoint == null)
                {
                    _timer.Start();
                    IsMouseEnter = false;
                    LineCanvas.Visibility = Visibility.Collapsed;
                    return;
                }

                RePositionLineAndEllipse(nearPoint);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            RestartTimer();
        }
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ResetCharts();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += OnTimerTick;
            DrawScales();
        }

        private void OnTimerTick(object? sender, System.EventArgs e)
        {
            if (ChartData.ContainsKey(_currentIndex) == false)
            {
                _timer.Stop();
                return;
            }
            var y = ChartData[_currentIndex];

            AddPoint(_currentIndex, y.Value);
            _currentIndex++;
        }

        #endregion

        #region Private methods

        private void AddPoint(double x, double y)
        {
            _chart.AddPoint(x, y);
            if (_chart.PointCount < 2)
                return;

            _chartResult = _chart.GetResult();

            MainChart.StartPoint = _chartResult.Points.First();

            var last = _chartResult.Points.Last();
            LastAddedValue = y;
            RePositionInfoBorder(last);

            PolyLineSegment.Points = new PointCollection(_chartResult.Points);
        }

        private void RestartTimer()
        {
            if (IsLoadData)
                _timer.Start();

            IsMouseEnter = false;
            LineCanvas.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region UI methods

        private void RePositionBorder(Point ellipsePosition)
        {
            int delta = 5;
            var borderHeight = TableBorder.Height;
            var borderWidth = TableBorder.Width;
            var xPos = ellipsePosition.X + EllipseInLine.Width / 2;
            var yPos = ellipsePosition.Y + EllipseInLine.Height / 2;
            if (xPos + borderWidth < _chartWidth)
                Canvas.SetLeft(TableBorder, xPos + delta);
            else
                Canvas.SetLeft(TableBorder, xPos - borderWidth - delta);

            if (yPos + borderHeight < _chartHeight)
                Canvas.SetTop(TableBorder, yPos + delta);
            else
                Canvas.SetTop(TableBorder, yPos - borderHeight - delta);
        }

        private void RePositionInfoBorder(Point pointPosition)
        {
            var yPos = pointPosition.Y - LastAddedBorder.Height / 2;
            Canvas.SetTop(LastAddedBorder, yPos);
        }

        private void RePositionLineAndEllipse((Point Point, double xAxis)? nearPoint)
        {
            VerticalLine.X1 = nearPoint.Value.Point.X;
            VerticalLine.X2 = nearPoint.Value.Point.X;
            VerticalLine.Y1 = _controlHeight - _startXPos / 2;
            VerticalLine.Y2 = _controlHeight - _chartHeight;
            HorizontalLine.X1 = _startXPos / 2;
            HorizontalLine.X2 = _chartWidth;
            HorizontalLine.Y1 = nearPoint.Value.Point.Y + _controlHeight - _chartHeight;
            HorizontalLine.Y2 = nearPoint.Value.Point.Y + _controlHeight - _chartHeight;

            var ellipsePosX = nearPoint.Value.Point.X - EllipseInLine.Width / 2;
            var ellipsePosY = nearPoint.Value.Point.Y - EllipseInLine.Height / 2 + _controlHeight - _chartHeight;

            Canvas.SetLeft(EllipseInLine, ellipsePosX);
            Canvas.SetTop(EllipseInLine, ellipsePosY);

            SelectValue = ChartData[(int)nearPoint.Value.xAxis].Value;
            SelectValueDate = ChartData[(int)nearPoint.Value.xAxis].ValueDate;

            RePositionBorder(new Point(ellipsePosX, ellipsePosY));

            LineCanvas.Visibility = Visibility.Visible;
        }

        private void DrawScales()
        {
            var stepYScale = _chartHeight / _scaleCount;
            var stepXScale = _chartWidth / _scaleCount;

            var index = 0;
            var pixels = _startXPos / 2;
            // Горизонтальные линии.
            while (index++ < _scaleCount - 1)
            {
                pixels += stepXScale;
                var line = new Line
                {
                    Stroke = (Brush)FindResource("WhiteBrush"),
                    StrokeThickness = 1,
                    Opacity = 0.1,
                    X1 = pixels,
                    X2 = pixels,
                    Y1 = _controlHeight - _startXPos / 2,
                    Y2 = _controlHeight - _chartHeight
                };

                ChartCanvas.Children.Add(line);

                var scaleLine = new Line
                {
                    Stroke = (Brush)FindResource("WhiteBrush"),
                    StrokeThickness = 1,
                    X1 = pixels,
                    X2 = pixels,
                    Y1 = _chartHeight + _startXPos / 2 - 5,
                    Y2 = _chartHeight + _startXPos / 2 + 5
                };
                ChartCanvas.Children.Add(scaleLine);
            }

            index = 0;
            pixels = _controlHeight - _chartHeight - _startXPos / 2;
            // Вертикальные линии.
            while (index++ < _scaleCount)
            {
                pixels += stepYScale;
                var line = new Line
                {
                    Stroke = (Brush)FindResource("WhiteBrush"),
                    StrokeThickness = 1,
                    Opacity = 0.1,
                    X1 = _startXPos / 2,
                    X2 = _chartWidth + _startXPos / 2,
                    Y1 = pixels,
                    Y2 = pixels
                };

                ChartCanvas.Children.Add(line);

                if (index != _scaleCount)
                {
                    var scaleLine = new Line
                    {
                        Stroke = (Brush)FindResource("WhiteBrush"),
                        StrokeThickness = 1,
                        X1 = _startXPos / 2 - 5,
                        X2 = _startXPos / 2 + 5,
                        Y1 = pixels,
                        Y2 = pixels
                    };
                    ChartCanvas.Children.Add(scaleLine);
                }
            }
        }

        private void ResetCharts()
        {
            _currentIndex = 0;
            _controlHeight = ChartCanvas.ActualHeight;
            _controlWidth = ChartCanvas.ActualWidth;

            _chartHeight = _controlHeight * _percentMargin / 100 - _startXPos / 2;
            _chartWidth = _controlWidth * _percentMargin / 100 - _startXPos / 2;

            var leftTop = new Point
            {
                X = _startXPos,
                Y = _controlHeight - _chartHeight
            };

            var rightBottom = new Point
            {
                X = _chartWidth,
                Y = _chartHeight
            };

            _chartResult = null;
            _chart = new Chart(leftTop, rightBottom, (x) => _chartHeight - x);
            IsLoadData = false;
        }


        #endregion
    }
}
