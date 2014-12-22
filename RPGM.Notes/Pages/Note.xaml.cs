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
            base.OnNavigatedFrom(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(0, 0, 0, 0);
            if (DataContext is ViewModel)
            {
                await ((ViewModel)DataContext).Initialize(e.Parameter);
            }
        }
    }
}
