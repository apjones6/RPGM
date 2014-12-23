using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace RPGM.Notes.Controls
{
    public class Flyout : Windows.UI.Xaml.Controls.Flyout
    {
        public static readonly DependencyProperty ClosedActionsProperty = DependencyProperty.Register("ClosedActions", typeof(ActionCollection), typeof(Flyout), new PropertyMetadata(null));
        public static readonly DependencyProperty OpenedActionsProperty = DependencyProperty.Register("OpenedActions", typeof(ActionCollection), typeof(Flyout), new PropertyMetadata(null));

        public Flyout()
        {
            SetValue(ClosedActionsProperty, new ActionCollection());
            SetValue(OpenedActionsProperty, new ActionCollection());
            Closed += OnClosed;
            Opened += OnOpened;
        }

        public ActionCollection ClosedActions
        {
            get { return (ActionCollection)GetValue(ClosedActionsProperty); }
        }

        public ActionCollection OpenedActions
        {
            get { return (ActionCollection)GetValue(OpenedActionsProperty); }
        }

        private void OnClosed(object sender, object e)
        {
            foreach (IAction action in ClosedActions)
            {
                action.Execute(sender, e);
            }
        }

        private void OnOpened(object sender, object e)
        {
            foreach (IAction action in OpenedActions)
            {
                action.Execute(sender, e);
            }
        }
    }
}
