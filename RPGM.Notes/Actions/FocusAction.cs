using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.Actions
{
    // NOTE: Unused as there are additional complications, such as separate visual trees for Top/Bottom bars
    public class FocusAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(Control), typeof(FocusAction), new PropertyMetadata((object)null));

        public Control TargetObject
        {
            get { return (Control)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }

        public object Execute(object sender, object parameter)
        {
            var control = TargetObject ?? sender as Control;
            if (control != null)
            {
                return control.Focus(FocusState.Programmatic);
            }

            return false;
        }
    }
}
