using System.Globalization;
using System.Windows;

namespace Charts.Converters
{
    /// <summary>
    /// Converter to hide a control when the Binding value is <see langword="false" />.
    /// </summary>
    public class BooleanToVisibilityConverter : BaseHybridConverterExtension
    {
        /// <summary>
        /// Convert from <see cref="bool" /> to <see cref="Visibility" />.
        /// </summary>
        public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (type != typeof(Visibility) && targetType != typeof(object))
                throw new ArgumentException($"{nameof(System.Convert)}: Incorrect type for targetType: {targetType.Name}");

            var bValue = value as bool? ?? false;
            if (bValue)
                return Visibility.Visible;

            var isHide = parameter as bool? ?? false;
            return isHide ? Visibility.Hidden : Visibility.Collapsed;
        }

        /// <summary>
        /// Convert from <see cref="Visibility" /> to <see cref="bool" />.
        /// </summary>
        public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            return value as Visibility? == Visibility.Visible;
        }

        /// <summary>
        /// Convert from <see cref="bool" /> to <see cref="Visibility" />.
        /// </summary>
        public override object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (type != typeof(Visibility) && targetType != typeof(object))
                throw new ArgumentException($"{nameof(System.Convert)}: Incorrect type for targetType: {targetType.Name}");

            foreach (var value in values)
            {
                var bValue = value as bool? ?? false;
                if (!bValue)
                {
                    var isHide = parameter as bool? ?? false;
                    return isHide ? Visibility.Hidden : Visibility.Collapsed;
                }
            }

            return Visibility.Visible;
        }
    }
}
