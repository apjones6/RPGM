using Windows.UI.Xaml;

namespace RPGM.Notes.Controls
{
    public class Flyout : Windows.UI.Xaml.Controls.Flyout
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new PropertyMetadata(false));

        public Flyout()
        {
            Closed += OnClosed;
            Opened += OnOpened;
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private void OnClosed(object sender, object e)
        {
            IsOpen = false;
        }

        private void OnOpened(object sender, object e)
        {
            IsOpen = true;
        }
    }
}
