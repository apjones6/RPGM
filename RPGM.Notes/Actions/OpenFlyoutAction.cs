using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace RPGM.Notes.Actions
{
    public class OpenFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var flyoutOwner = sender as FrameworkElement;
            if (flyoutOwner != null)
            {
                FlyoutBase.ShowAttachedFlyout(flyoutOwner);
                return true;
            }

            return false;
        }
    }
}
