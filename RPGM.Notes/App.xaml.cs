﻿using System;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using RPGM.Notes.Messages;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes
{
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        public App()
        {
            // TODO: Investigate whether we need to detach and reattach this on lifecycle events
            HardwareButtons.BackPressed += OnBackPressed;

            this.InitializeComponent();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            // Allow view models to handle, such as cancel multiple selection mode
            var message = new BackMessage();
            Messenger.Default.Send<BackMessage>(message);
            if (message.Handled)
            {
                e.Handled = true;
                return;
            }

            // Allow frame to handle
            var frame = Window.Current.Content as Frame;
            if (frame != null && frame.CanGoBack)
            {
                e.Handled = true;
                frame.GoBack();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = rootFrame.ContentTransitions;
                    rootFrame.ContentTransitions = null;
                }

                rootFrame.Navigated += this.OnNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(Pages.Main), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // NOTE: We only need this if our app is using this class
            DispatcherHelper.Initialize();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.OnNavigated;
        }
    }
}