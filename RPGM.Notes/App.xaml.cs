using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using RPGM.Notes.Models;
using RPGM.Notes.ViewModels;
using RPGM.Notes.Views;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RPGM.Notes
{
    public sealed partial class App : MvvmAppBase
    {
        private static readonly Regex VIEW_TO_VIEWMODEL = new Regex(@"\.Views\.([a-zA-Z0-9_]+)Page,");

        private CompositionHost container;

        public App()
        {
            InitializeComponent();
        }

        private void OnDeleteAsync(IEnumerable<Guid> ids)
        {
            var frame = (Frame)Window.Current.Content;
            var session = SessionStateService.GetSessionStateForFrame(new FrameFacadeAdapter(frame));
            Guid id;

            // Remove pages for deleted notes from the back stack, so that they are skipped
            for (var index = frame.BackStackDepth - 1; index >= 0; index--)
            {
                var entry = frame.BackStack[index];
                if (entry.SourcePageType == typeof(NotePage) && entry.Parameter != null && Guid.TryParse(entry.Parameter.ToString(), out id) && ids.Contains(id))
                {
                    RemoveSessionState(session, index);
                    frame.BackStack.Remove(entry);
                }
            }
        }

        protected override void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            var page = (Page)frame.Content;
            var backAware = page.DataContext as IBackNavigationAware;
            if (backAware != null)
            {
                if (!backAware.TryGoBack())
                {
                    e.Handled = true;
                }
            }

            // NOTE: Setting e.Handled should be enough but it isn't
            if (!e.Handled)
            {
                base.OnHardwareButtonsBackPressed(sender, e);
            }
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var eventAggregator = new EventAggregator();

            // NOTE: Create event aggregator here, to ensure it has access to the UI thread synchronization context
            container = new ContainerConfiguration()
                .WithAssembly(typeof(App).GetTypeInfo().Assembly)
                .WithInstance<IEventAggregator>(eventAggregator)
                .WithInstance<INavigationService>(NavigationService)
                .WithInstance<ISessionStateService>(SessionStateService)
                .CreateContainer();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(x => Type.GetType(VIEW_TO_VIEWMODEL.Replace(x.AssemblyQualifiedName, ".ViewModels.$1ViewModel,")));

            eventAggregator.GetEvent<DeleteEvent>().Subscribe(OnDeleteAsync, ThreadOption.UIThread);

            return Task.FromResult(0);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            // TODO: Test if we need to check previous execution state here
            NavigationService.Navigate("NoteList", null);
            return Task.FromResult(0);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            SessionStateService.RegisterKnownType(typeof(Note));
        }

        private void RemoveSessionState(Dictionary<string, object> state, int index)
        {
            var i = index + 1;
            while (state.ContainsKey("ViewModel-" + i))
            {
                state["ViewModel-" + (i - 1)] = state["ViewModel-" + i];
                i++;
            }

            i = index + 1;
            while (state.ContainsKey("Page-" + i))
            {
                state["Page-" + (i - 1)] = state["Page-" + i];
                i++;
            }
        }

        protected override object Resolve(Type type)
        {
            return container.GetExport(type);
        }
    }
}