using System.Windows;

namespace Charts
{
    /// <summary>
    /// Button extensions.
    /// </summary>
    public static class ButtonExtensions
    {
        #region Corner radius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ButtonExtensions), new UIPropertyMetadata(new CornerRadius(0)));

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        #endregion
    }
}
