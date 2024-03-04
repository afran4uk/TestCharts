using System.Globalization;
using System.Windows.Data;

namespace Charts.Converters
{
    public class AddNumericValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualSize && parameter is string valueString)
            {
                if (double.TryParse(valueString, out double valueDouble))
                {
                    return actualSize + valueDouble;
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
