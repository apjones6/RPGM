using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public abstract class ViewModel : ViewModelBase
    {
        private readonly INavigationService navigation;
        private readonly IDatabase database;

        protected ViewModel()
        {
        }

        protected ViewModel(INavigationService navigation, IDatabase database)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (database == null) throw new ArgumentNullException("database");

            this.navigation = navigation;
            this.database = database;
        }

        protected IDatabase Database
        {
            get { return database; }
        }

        protected IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        protected INavigationService Navigation
        {
            get { return navigation; }
        }

        public virtual Task InitializeAsync(object parameter)
        {
            return Task.FromResult(0);
        }
    }
}
