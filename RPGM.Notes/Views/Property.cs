using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.Views
{
    public static class Property
    {
        public static readonly DependencyProperty RichTextProperty = DependencyProperty.RegisterAttached("RichText", typeof(string), typeof(Property), PropertyMetadata.Create(OnCreateDefaultRichText, OnRichTextChanged));

        public static string GetRichText(DependencyObject obj)
        {
            return (string)obj.GetValue(RichTextProperty);
        }

        public static void SetRichText(DependencyObject obj, string value)
        {
            obj.SetValue(RichTextProperty, value);
        }

        private static object OnCreateDefaultRichText()
        {
            string value;
            new RichEditBox().Document.GetText(TextGetOptions.FormatRtf, out value);
            return value;
        }

        private static void OnRichTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichEditBox)d).Document.SetText(TextSetOptions.FormatRtf, (string)e.NewValue);
        }
    }
}
