using RPGM.Notes.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Pages
{
    public sealed partial class Note : Page
    {
        public Note()
        {
            this.InitializeComponent();

            // Ideally we use Behaviors for this, but they don't work for Flyouts
            // Furthermore extending Flyout with special ActionCollections we next hit issue with the Top/Bottom bars running on separate visual trees
            this.FormatFlyout.Closed += OnFlyoutClosed;
            this.FormatFlyout.Opened += OnFlyoutOpened;
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            BottomAppBar.Visibility = Visibility.Visible;
            RtfContentBox.Focus(FocusState.Programmatic);
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            BottomAppBar.Visibility = Visibility.Collapsed;
            BottomAppBar.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StatusBar.GetForCurrentView().ForegroundColor = null;
            InputPane.GetForCurrentView().Showing -= OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Hiding -= OnOccludedRectUpdate;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(0, 0, 0, 0);
            InputPane.GetForCurrentView().Showing += OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Hiding += OnOccludedRectUpdate;

            if (DataContext is NoteViewModel)
            {
                await ((NoteViewModel)DataContext).InitializeAsync(e.Parameter, RtfContentBox.Document);
            }
        }

        private void OnOccludedRectUpdate(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            KeyboardPlaceholder.Height = sender.OccludedRect.Height;
        }
    }
}
