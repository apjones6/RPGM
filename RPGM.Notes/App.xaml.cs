using System;
using Caliburn.Micro;
using RPGM.Notes.Controls;
using RPGM.Notes.Models;
using RPGM.Notes.ViewModels;
using RPGM.Notes.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes
{
    public sealed partial class App : CaliburnApplication
    {
        private WinRTContainer container;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            container = new WinRTContainer();

            // Register UI values
            MessageBinder.SpecialValues.Add("$navigateuri", c => ((NavigationEventArgs)c.EventArgs).Uri);

            // Register components
            container.RegisterWinRTServices();
            container.Singleton<IDatabase, Database>();
            container.PerRequest<MainViewModel>();
            container.PerRequest<NoteViewModel>();

            PrepareViewFirst();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance == null)
            {
                throw new Exception("Could not locate any instances.");
            }

            return instance;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                // NOTE: ViewModel-first approach here causes issues with navigation and back pressed events
                DisplayRootView<MainView>();
            }
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
        }
    }
}