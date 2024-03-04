using System.Windows;

namespace Charts.Controls
{
    public class ChartResult(SortedDictionary<double, double> axis, Point[] points, Dictionary<Point, (double, double)> xPointToAxis)
    {
        public IReadOnlyDictionary<double, double> Axis = axis;
        public Dictionary<Point, (double, double)> XPointToAxis = xPointToAxis;
        public readonly Point[] Points = points;
    }

    public class Chart
    {
        private readonly Func<double, double>? _yAxisConverter;
        private readonly Point _leftTop;
        private readonly Point _rightBottom;
        private SortedDictionary<double, double> _axis = new();
        private List<Point> _points = new();
        
        private Dictionary<Point, (double, double)> _pointToAxis;

        public int PointCount => _points.Count;

        public Chart(Point leftTop, Point rightBottom, Func<double, double>? yAxisConverter = null)
        {
            _yAxisConverter = yAxisConverter;
            _leftTop = leftTop with { Y = yAxisConverter?.Invoke(leftTop.Y) ?? leftTop.Y };
            _rightBottom = rightBottom with { Y = yAxisConverter?.Invoke(rightBottom.Y) ?? rightBottom.Y };
        }

        public void AddPoint(double x, double y)
        {
            _axis.Add(x, y);

            var yMax = _axis.Values.Max();
            var yMin = _axis.Values.Min();

            var yAxisLength = _leftTop.Y - _rightBottom.Y;
            var xAxisLength = _rightBottom.X - _leftTop.X;

            var xStep = xAxisLength / (_axis.Values.Count - 1);
            var yStep = yAxisLength / (yMax - yMin);

            _points = new List<Point>();
            _pointToAxis = new Dictionary<Point, (double, double)>();
            
            foreach (var (key, value) in _axis)
            {
                var point = new Point
                {
                    X = _leftTop.X + (key - _axis.Keys.Min()) * xStep,
                    Y = _leftTop.Y - (value - yMin) * yStep
                };

                _points.Add(point);
                _pointToAxis.Add(point, (key, value));
            }
        }

        public (Point Point, double xAxis)? GetNearPoint(int xPos)
        {
            var chartResult = GetResult();
            for (int i = 0; i < chartResult.XPointToAxis.Count - 1; i++)
            {
                var prevElement = chartResult.XPointToAxis.ElementAt(i);
                var nextElement = chartResult.XPointToAxis.ElementAt(i + 1);

                if (xPos > prevElement.Key.X && xPos < nextElement.Key.X)
                {
                    var absPrev = Math.Abs(xPos - prevElement.Key.X);
                    var absNext = Math.Abs(xPos - nextElement.Key.X);
                    return absNext > absPrev
                        ? (prevElement.Key, prevElement.Value.Item1)
                        : (nextElement.Key, nextElement.Value.Item1);
                }
            }

            return null;
        }

        public ChartResult GetResult()
        {
            var convertedPoints = _points.Select(p => new Point
            {
                X = p.X,
                Y = _yAxisConverter?.Invoke(_leftTop.Y - p.Y) ?? p.Y
            }).ToArray();

            return new ChartResult(_axis, convertedPoints, _pointToAxis);
        }
    }
}
