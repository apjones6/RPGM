using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class SelectionChangedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((ListViewBase)parameter).SelectedItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
