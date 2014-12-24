using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class BooleanToUIConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolean = value != null && value.Equals(true);

            if (IsInverted(parameter))
            {
                boolean = !boolean;
            }

            if (targetType == typeof(Visibility))
            {
                return boolean ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (targetType == typeof(ListViewSelectionMode))
            {
                return boolean ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
            }

            return boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var boolean = false;

            if (value is Visibility)
            {
                boolean = (Visibility)value == Visibility.Visible;
            }
            else if (value is ListViewSelectionMode)
            {
                boolean = (ListViewSelectionMode)value == ListViewSelectionMode.Multiple;
            }

            if (IsInverted(parameter))
            {
                boolean = !boolean;
            }

            return boolean;
        }

        private static bool IsInverted(object parameter)
        {
            return parameter is string && "invert".Equals((string)parameter, StringComparison.OrdinalIgnoreCase);
        }
    }
}
