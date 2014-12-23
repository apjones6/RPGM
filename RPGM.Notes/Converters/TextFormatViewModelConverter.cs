using System;
using RPGM.Notes.ViewModels;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    // NOTE: Unused as there are additional complications, such as separate visual trees for Top/Bottom bars
    public class TextFormatViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ITextDocument)
            {
                return new TextFormatViewModel((ITextDocument)value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TextFormatViewModel)
            {
                return ((TextFormatViewModel)value).Document;
            }

            return value;
        }
    }
}
