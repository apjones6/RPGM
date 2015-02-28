using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Views
{
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            // NOTE: This prevents list reloading on clicking back (maintains scroll position etc) but
            //       stops new notes appearing and breaks delete selected binding
            //this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }
    }
}
