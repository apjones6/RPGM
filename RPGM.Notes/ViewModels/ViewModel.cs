using System;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.UI;
using Windows.UI.Xaml;

namespace RPGM.Notes.ViewModels
{
    public abstract class ViewModel : Screen
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

        protected static Color AccentColor
        {
            get { return (Color)Application.Current.Resources["SystemColorHighlightColor"]; }
        }

        protected IDatabase Database
        {
            get { return database; }
        }

        protected INavigationService Navigation
        {
            get { return navigation; }
        }
    }
}
