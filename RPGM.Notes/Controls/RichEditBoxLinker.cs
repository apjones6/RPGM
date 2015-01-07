using System;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RPGM.Notes.Controls
{
    public class RichEditBoxLinker : Canvas
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(RichEditBoxLinker), new PropertyMetadata(true, IsEnabledCallback));
        public static readonly DependencyProperty RichEditBoxProperty = DependencyProperty.Register("RichEditBox", typeof(RichEditBox), typeof(RichEditBoxLinker), new PropertyMetadata(null));

        public RichEditBoxLinker()
        {
            UpdateIsEnabledRelatedProperties(this, true);
            Tapped += OnTapped;
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public RichEditBox RichEditBox
        {
            get { return (RichEditBox)GetValue(RichEditBoxProperty); }
            set { SetValue(RichEditBoxProperty, value); }
        }

        public event EventHandler<NavigationEventArgs> Navigate;

        private static void IsEnabledCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateIsEnabledRelatedProperties((RichEditBoxLinker)d, (bool)e.NewValue);
        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            // Uses screen coordinates, as I had some inconsistency converting between control relative coordinates
            var position = e.GetPosition(null);
            var range = RichEditBox.Document.GetRangeFromPoint(position, PointOptions.None);
            range.StartOf(TextRangeUnit.Link, true);

            if (!string.IsNullOrEmpty(range.Link))
            {
                // Trim wrapping quotes
                // TODO: Robustness
                var uri = new Uri(range.Link.Substring(1, range.Link.Length - 2));
                
                // Dispatch event if any listeners, else launch normally
                if (Navigate != null)
                {
                    Navigate(this, new NavigationEventArgs(uri));
                }
                else
                {
                    await Launcher.LaunchUriAsync(uri);
                }
            }
        }

        private static void UpdateIsEnabledRelatedProperties(RichEditBoxLinker linker, bool isEnabled)
        {
            linker.Background = isEnabled ? new SolidColorBrush(new Color { A = 0x00 }) : null;
            linker.IsHitTestVisible = isEnabled;
            linker.IsTapEnabled = isEnabled;
        }
    }

    public class NavigationEventArgs : EventArgs
    {
        private readonly Uri uri;

        public NavigationEventArgs(Uri uri)
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
