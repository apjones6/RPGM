using System;
using System.Composition.Hosting;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using RPGM.Notes.Models;
using RPGM.Notes.ViewModels;
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
            container = new ContainerConfiguration()
                .WithAssembly(typeof(App).GetTypeInfo().Assembly)
                .WithInstance<IEventAggregator>(new EventAggregator())
                .WithInstance<INavigationService>(NavigationService)
                .CreateContainer();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelTypeResolver);

            return Task.FromResult(0);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            // TODO: Test if we need to check previous execution state here
            NavigationService.Navigate("Main", null);
            return Task.FromResult(0);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            SessionStateService.RegisterKnownType(typeof(Note));
        }

        protected override object Resolve(Type type)
        {
            return container.GetExport(type);
        }

        private static Type ViewTypeToViewModelTypeResolver(Type view)
        {
            var viewmodel = VIEW_TO_VIEWMODEL.Replace(view.AssemblyQualifiedName, ".ViewModels.$1ViewModel,");
            return Type.GetType(viewmodel);
        }
    }
}