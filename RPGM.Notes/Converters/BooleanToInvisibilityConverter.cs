using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Collapsed;
        }
    }
}
