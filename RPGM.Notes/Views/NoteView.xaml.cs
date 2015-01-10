using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace RPGM.Notes.Views
{
    public sealed partial class NoteView : Page
    {
        public NoteView()
        {
            this.InitializeComponent();
        }

        private void OnCanvasTapped(object sender, TappedRoutedEventArgs e)
        {
            // TODO: MVVM if acceptable
            // TODO: Resolve intermittent occluded rect above keyboard (custom keyboard handler, or animate occluded rect perhaps)
            RtfContentBox.Document.Selection.SetPoint(e.GetPosition(null), PointOptions.None, false);
            RtfContentBox.Document.Selection.ScrollIntoView(PointOptions.None);
            RtfContentBox.Focus(FocusState.Pointer);
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            // Ideally we use Behaviors for this, but they don't work for Flyouts
            // Furthermore extending Flyout with special ActionCollections we next hit issue with the Top/Bottom bars running on separate visual trees
            RtfContentBox.Focus(FocusState.Programmatic);
        }
    }
}
