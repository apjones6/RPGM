using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.Controls
{
    public class OcclusionPlaceholder : Control
    {
        public OcclusionPlaceholder()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Hiding += OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Showing += OnOccludedRectUpdate;
        }

        private void OnOccludedRectUpdate(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            Height = e.OccludedRect.Height;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            InputPane.GetForCurrentView().Hiding -= OnOccludedRectUpdate;
            InputPane.GetForCurrentView().Showing -= OnOccludedRectUpdate;
        }
    }
}
