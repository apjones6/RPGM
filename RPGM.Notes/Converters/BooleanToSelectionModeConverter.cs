using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                return (bool)value ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is ListViewSelectionMode)
            {
                return (ListViewSelectionMode)value == ListViewSelectionMode.Multiple;
            }

            return value;
        }
    }
}
