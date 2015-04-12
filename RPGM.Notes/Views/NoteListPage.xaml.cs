using Microsoft.Practices.Prism.StoreApps;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Views
{
    public sealed partial class NoteListPage : VisualStateAwarePage
    {
        public NoteListPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = null;
        }
    }
}
