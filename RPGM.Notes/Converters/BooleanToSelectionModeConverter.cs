using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (ListViewSelectionMode)value == ListViewSelectionMode.Multiple;
        }
    }
}
