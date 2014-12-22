﻿using RPGM.Notes.Controls;
using RPGM.Notes.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.Pages
{
    public sealed partial class Note : Page
    {
        public Note()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StatusBarHelper.ForegroundColor = StatusBarHelper.COLOR_BLACK;
            if (DataContext is ViewModel)
            {
                await ((ViewModel)DataContext).Initialize(e.Parameter);
            }
        }
    }
}
