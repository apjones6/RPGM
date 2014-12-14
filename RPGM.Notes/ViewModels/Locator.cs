using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace RPGM.Notes.ViewModels
{
    public class Locator : IDisposable
    {
        public Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModel.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<INavigationService, NavigationService>();
            }
            else
            {
                SimpleIoc.Default.Register<INavigationService>(CreateNavigationService);
            }

            SimpleIoc.Default.Register<NotesViewModel>();
            SimpleIoc.Default.Register<NoteViewModel>();
        }

        public NotesViewModel Notes
        {
            get { return ServiceLocator.Current.GetInstance<NotesViewModel>(); }
        }

        public NoteViewModel Note
        {
            get { return ServiceLocator.Current.GetInstance<NoteViewModel>(); }
        }
        
        public void Dispose()
        {
            SimpleIoc.Default.Reset();
        }

        private static NavigationService CreateNavigationService()
        {
            var navigation = new NavigationService();

            navigation.Configure("Main", typeof(Pages.Main));
            navigation.Configure("Rename", typeof(Pages.Rename));

            return navigation;
        }
    }
}