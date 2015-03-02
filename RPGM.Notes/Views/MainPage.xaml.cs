using Microsoft.Practices.Prism.StoreApps;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Views
{
    public sealed partial class MainPage : VisualStateAwarePage
    {
        public MainPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = null;
            base.OnNavigatedTo(e);
        }
    }
}
