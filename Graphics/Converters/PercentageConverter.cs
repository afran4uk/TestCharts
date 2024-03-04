using System.Globalization;
using System.Windows.Data;

namespace Charts.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualSize && parameter is string percentString)
            {
                if (double.TryParse(percentString, out double percentage))
                {
                    return actualSize * (percentage / 100.0);
                }
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
