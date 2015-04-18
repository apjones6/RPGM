using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null && value.Equals(0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Throw as there's no way we can safely convert true to the original value
            throw new NotSupportedException();
        }
    }
}
