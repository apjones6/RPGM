using System;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanNegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }
    }
}
