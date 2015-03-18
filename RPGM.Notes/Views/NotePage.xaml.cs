using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using RPGM.Notes.ViewModels;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RPGM.Notes.Views
{
    public sealed partial class NotePage : VisualStateAwarePage
    {
        private static readonly Color COLOR_BLACK = Color.FromArgb(0, 0, 0, 0);

        private readonly ICommand goHome;

        public NotePage()
        {
            // NOTE: We can't cache this page for a couple reasons, but a key one is that it prevents continuum transitions
            this.goHome = new DelegateCommand(() => GoHome(null, null));
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        public ICommand GoHomeCommand
        {
            get { return goHome; }
        }

        private void OnCanvasTapped(object sender, TappedRoutedEventArgs e)
        {
            // TODO: MVVM if acceptable - Use Behaviours/Actions
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StatusBar.GetForCurrentView().ForegroundColor = COLOR_BLACK;
            var vm = DataContext as NoteViewModel;
            if (vm != null)
            {
                vm.SetDocument(RtfContentBox.Document);
            }
        }
    }
}
