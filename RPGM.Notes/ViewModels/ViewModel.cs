using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Models;

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

        protected State State
        {
            get { return State.Current; }
        }

        public virtual Task Initialize(object parameter)
        {
            return Task.FromResult(0);
        }
    }
}
