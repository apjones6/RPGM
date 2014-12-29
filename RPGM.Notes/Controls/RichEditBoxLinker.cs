using System;
using System.Windows.Input;
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
        public static readonly DependencyProperty NavigationCommandProperty = DependencyProperty.Register("NavigationCommand", typeof(ICommand), typeof(RichEditBoxLinker), new PropertyMetadata(null));
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

        public ICommand NavigationCommand
        {
            get { return (ICommand)GetValue(NavigationCommandProperty); }
            set { SetValue(NavigationCommandProperty, value); }
        }

        public RichEditBox RichEditBox
        {
            get { return (RichEditBox)GetValue(RichEditBoxProperty); }
            set { SetValue(RichEditBoxProperty, value); }
        }

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
                
                // Use command if able, else launch normally
                if (NavigationCommand != null && NavigationCommand.CanExecute(uri))
                {
                    NavigationCommand.Execute(uri);
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
}
