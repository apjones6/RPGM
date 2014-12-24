using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class SelectionChangedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // NOTE: Very questionable whether these two scenarios should be handled in this class, simply
            //       because of the event name being the same.
            if (parameter is ListViewBase)
            {
                return ((ListViewBase)parameter).SelectedItems;
            }
            else if (parameter is RichEditBox)
            {
                return ((RichEditBox)parameter).Document.Selection;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
