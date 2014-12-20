﻿using RPGM.Notes.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Pages
{
    public sealed partial class Main : Page
    {
        public Main()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext is ViewModel)
            {
                await ((ViewModel)DataContext).Initialize(e.Parameter);
            }
        }
    }
}