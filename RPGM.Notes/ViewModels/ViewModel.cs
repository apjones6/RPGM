using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace RPGM.Notes.ViewModels
{
    public abstract class ViewModel : ViewModelBase
    {
        private readonly INavigationService navigation;

        public ViewModel(INavigationService navigation)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            this.navigation = navigation;
        }

        protected INavigationService Navigation
        {
            get { return navigation; }
        }

        public virtual void Initialize(object parameter)
        {
        }
    }
}
