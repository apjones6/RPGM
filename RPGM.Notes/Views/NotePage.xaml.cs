using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.StoreApps;
using RPGM.Notes.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

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
        }

        public ICommand GoHomeCommand
        {
            get { return goHome; }
        }

        private void OnFlyoutClosed(object sender, object e)
        {
            // Ideally we use Behaviors for this, but they don't work for Flyouts
            // Furthermore extending Flyout with special ActionCollections we next hit issue with the Top/Bottom bars running on separate visual trees
            RtfContentBox.Focus(FocusState.Programmatic);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;
            StatusBar.GetForCurrentView().ForegroundColor = COLOR_BLACK;
            if (DataContext is IDocumentAware)
            {
                ((IDocumentAware)DataContext).SetDocument(RtfContentBox.Document);
            }

            // Focus the title textbox if it's editable
            if (TitleBox.IsEnabled)
            {
                TitleBox.Focus(FocusState.Programmatic);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var pane = InputPane.GetForCurrentView();
            pane.Showing -= OnOccludedRectUpdate;
            pane.Hiding -= OnOccludedRectUpdate;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var pane = InputPane.GetForCurrentView();
            pane.Showing += OnOccludedRectUpdate;
            pane.Hiding += OnOccludedRectUpdate;
            Loaded += OnLoaded;
        }

        private void OnOccludedRectUpdate(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            ScrollViewer.Margin = new Thickness(0, 0, 0, e.OccludedRect.Height);
        }
    }
}
