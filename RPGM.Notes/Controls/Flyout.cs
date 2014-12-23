using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace RPGM.Notes.Controls
{
    // NOTE: Unused as there are additional complications, such as separate visual trees for Top/Bottom bars
    public class Flyout : Windows.UI.Xaml.Controls.Flyout
    {
        public static readonly DependencyProperty ClosedActionsProperty = DependencyProperty.Register("ClosedActions", typeof(ActionCollection), typeof(Flyout), PropertyMetadata.Create((object)null));
        public static readonly DependencyProperty OpenedActionsProperty = DependencyProperty.Register("OpenedActions", typeof(ActionCollection), typeof(Flyout), PropertyMetadata.Create((object)null));

        public Flyout()
        {
            Closed += OnClosed;
            Opened += OnOpened;
        }

        public ActionCollection ClosedActions
        {
            get { return (ActionCollection)GetValue(ClosedActionsProperty); }
            set { SetValue(ClosedActionsProperty, value); }
        }

        public ActionCollection OpenedActions
        {
            get { return (ActionCollection)GetValue(OpenedActionsProperty); }
            set { SetValue(OpenedActionsProperty, value); }
        }

        private void OnClosed(object sender, object e)
        {
            foreach (var action in ClosedActions)
            {
                ((IAction)action).Execute(sender, e);
            }
        }

        private void OnOpened(object sender, object e)
        {
            foreach (var action in OpenedActions)
            {
                ((IAction)action).Execute(sender, e);
            }
        }
    }
}
