using System;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RPGM.Notes.Views
{
    public sealed partial class LinkRichEditBox : UserControl
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(ITextDocument), typeof(LinkRichEditBox), new PropertyMetadata(null));
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register("IsFocused", typeof(bool), typeof(LinkRichEditBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(LinkRichEditBox), new PropertyMetadata(false, OnIsReadOnlyChanged));
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(LinkRichEditBox), new PropertyMetadata(null));

        private static Brush TRANSPARENT_BRUSH = new SolidColorBrush(new Color { A = 0x00 });

        public LinkRichEditBox()
        {
            this.InitializeComponent();

            SetValue(DocumentProperty, EditBox.Document);

            EditBox.GotFocus += OnEditBoxGotFocus;
            EditBox.LostFocus += OnEditBoxLostFocus;
            LinkCanvas.Tapped += OnLinkCanvasTapped;
        }

        public ITextDocument Document
        {
            get { return (ITextDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { throw new NotSupportedException(); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public event EventHandler<UriNavigationEventArgs> Navigate;

        private void OnEditBoxGotFocus(object sender, RoutedEventArgs e)
        {
            SetValue(IsFocusedProperty, true);
        }

        private void OnEditBoxLostFocus(object sender, RoutedEventArgs e)
        {
            SetValue(IsFocusedProperty, false);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            // TODO: Find out how to effectively propagate focus to the appropriate subcontrol
            EditBox.Focus(FocusState.Programmatic);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (LinkRichEditBox)d;
            if (box.IsReadOnly)
            {
                box.LinkCanvas.Background = TRANSPARENT_BRUSH;
                box.LinkCanvas.IsHitTestVisible = true;
                box.LinkCanvas.IsTapEnabled = true;
            }
            else
            {
                box.LinkCanvas.Background = null;
                box.LinkCanvas.IsHitTestVisible = false;
                box.LinkCanvas.IsTapEnabled = false;
            }
        }

        private async void OnLinkCanvasTapped(object sender, TappedRoutedEventArgs e)
        {
            // Uses screen coordinates, as I had some inconsistency converting between control relative coordinates
            var uri = EditBox.Document.GetLinkFromPoint(e.GetPosition(null), PointOptions.None);
            if (uri != null)
            {
                // Dispatch event if any listeners, else launch normally
                if (Navigate != null)
                {
                    Navigate(this, new UriNavigationEventArgs(uri));
                }
                else
                {
                    await Launcher.LaunchUriAsync(uri);
                }
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (!IsReadOnly)
            {
                // TODO: Resolve intermittent occluded rect above keyboard (custom keyboard handler, or animate occluded rect perhaps)
                EditBox.Document.Selection.SetPoint(e.GetPosition(null), PointOptions.None, false);
                EditBox.Document.Selection.ScrollIntoView(PointOptions.None);
                EditBox.Focus(FocusState.Pointer);
            }
        }
    }

    public class UriNavigationEventArgs : EventArgs
    {
        private readonly Uri uri;

        public UriNavigationEventArgs(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            this.uri = uri;
        }

        public Uri Uri
        {
            get { return uri; }
        }
    }
}
