using System;
using Windows.UI.Text;
using Windows.UI.Xaml;

namespace RPGM.Notes.Views
{
    public class RichEditBox : Windows.UI.Xaml.Controls.RichEditBox
    {
        public static readonly DependencyProperty RtfTextProperty = DependencyProperty.Register("RtfText", typeof(string), typeof(RichEditBox), PropertyMetadata.Create(string.Empty, OnRtfTextChanged));

        public RichEditBox()
        {
            TextChanged += OnTextChanged;
        }

        public string RtfText
        {
            get { return (string)GetValue(RtfTextProperty); }
            set { SetValue(RtfTextProperty, value); }
        }

        private static void OnRtfTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as RichEditBox;
            if (box != null)
            {
                IfIdle(box, () => box.Document.SetText(TextSetOptions.FormatRtf, box.RtfText));
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            IfIdle(this, () =>
                {
                    string value;
                    Document.GetText(TextGetOptions.None, out value);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        RtfText = string.Empty;
                    }
                    else
                    {
                        Document.GetText(TextGetOptions.FormatRtf, out value);
                        RtfText = value;
                    }
                });
        }

        private bool idle = true;
        private static void IfIdle(RichEditBox box, Action action)
        {
            if (box.idle)
            {
                box.idle = false;
                action();
                box.idle = true;
            }
        }
    }
}
