using RPGM.Notes.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = null;
            InputPane.GetForCurrentView().Showing -= OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Hiding -= OnOccludedRectUpdate;
            base.OnNavigatedFrom(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(0, 0, 0, 0);
            InputPane.GetForCurrentView().Showing += OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Hiding += OnOccludedRectUpdate;

            if (DataContext is ViewModel)
            {
                await ((ViewModel)DataContext).Initialize(e.Parameter);
            }
        }

        private void OnOccludedRectUpdate(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            KeyboardPlaceholder.Height = sender.OccludedRect.Height;
        }
    }
}
