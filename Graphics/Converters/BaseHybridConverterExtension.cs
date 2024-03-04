using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Charts.Converters
{
    /// <summary>
    /// A base class for creating converters that supports both single-parameter conversion and conversion of multiple parameters.
    /// </summary>
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public abstract class BaseHybridConverterExtension : MarkupExtension, IMultiValueConverter, IValueConverter
    {
        /// <summary>
        /// A method that allows declaring the converter as a MarkupExtension.
        /// </summary>
        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc />
        public virtual object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
