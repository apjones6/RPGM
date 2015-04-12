using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes.Views
{
    public sealed partial class TextFormatFlyout : Flyout
    {
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), typeof(TextFormatFlyout), new PropertyMetadata(null, OnDataContextChanged));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new PropertyMetadata(false));

        public TextFormatFlyout()
        {
            this.InitializeComponent();
            Closed += OnClosed;
            Opened += OnOpened;
        }

        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
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

        private static void OnDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = ((Flyout)sender).Content as FrameworkElement;
            if (element != null)
            {
                element.DataContext = e.NewValue;
            }
        }

        private void OnOpened(object sender, object e)
        {
            IsOpen = true;
        }
    }
}
