using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return (IsInverted(parameter) ? !(bool)value : (bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility)
            {
                return IsInverted(parameter) ? (Visibility)value != Visibility.Visible : (Visibility)value == Visibility.Visible;
            }

            return value;
        }

        private static bool IsInverted(object parameter)
        {
            return parameter is string && "invert".Equals((string)parameter, StringComparison.OrdinalIgnoreCase);
        }
    }
}
