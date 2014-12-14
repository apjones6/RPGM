using RPGM.Notes.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Pages
{
    public sealed partial class Rename : Page
    {
        public Rename()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext is ViewModel)
            {
                ((ViewModel)DataContext).Initialize(e.Parameter);
            }
        }
    }
}
