using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using RPGM.Notes.Models;
using RPGM.Notes.ViewModels;

namespace RPGM.Notes
{
    public class Injector : ICleanup
    {
        public Injector()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<INavigationService, RPGMNavigationService>();
            SimpleIoc.Default.Register<IDatabase, Database>();
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

        public void Cleanup()
        {
            SimpleIoc.Default.Reset();
        }

        public class RPGMNavigationService : NavigationService
        {
            public RPGMNavigationService()
            {
                Configure("Main", typeof(Pages.Main));
                Configure("Note", typeof(Pages.Note));
                Configure("Rename", typeof(Pages.Rename));
            }
        }
    }
}