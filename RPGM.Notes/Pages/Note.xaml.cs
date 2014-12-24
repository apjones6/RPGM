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
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            // Ideally we use Behaviors for this, but they don't work for Flyouts
            // Furthermore extending Flyout with special ActionCollections we next hit issue with the Top/Bottom bars running on separate visual trees
            RtfContentBox.Focus(FocusState.Programmatic);
        }

        private void OnFlyoutOpened(object sender, object e)
        {
            // Ideally we use Behaviors for this, but they don't work for Flyouts
            // Furthermore extending Flyout with special ActionCollections we next hit issue with the Top/Bottom bars running on separate visual trees
            BottomAppBar.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StatusBar.GetForCurrentView().ForegroundColor = null;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(0, 0, 0, 0);
            if (DataContext is ViewModel)
            {
                await ((ViewModel)DataContext).InitializeAsync(e.Parameter);
            }
        }
    }
}
