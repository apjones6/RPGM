using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.Views
{
    public sealed partial class NoteView : Page
    {
        public NoteView()
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
    }
}
