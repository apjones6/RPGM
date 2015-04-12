using System;
using System.Diagnostics;
using Microsoft.Xaml.Interactivity;
using RPGM.Notes.Controls;
using RPGM.Notes.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RPGM.Notes.Actions
{
    public class NavigateToNoteAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var args = parameter as NavigationEventArgs;
            if (args == null)
            {
                return false;
            }

            var parts = args.Uri.AbsoluteUri.ToLower().Replace("richtea.rpgm://", string.Empty).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var id = parts.Length > 1 ? parts[1] : null;

            if (parts[0] == "notes")
            {
                return GetFrame((DependencyObject)sender).Navigate(typeof(NotePage), id);
            }
            else
            {
                Debug.WriteLine("Unrecognized URI: {0}", args.Uri);
                // TODO: Throw?
            }

            return false;
        }

        private static Frame GetFrame(DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            var frame = parent as Frame;
            if (frame == null)
            {
                return GetFrame(parent);
            }

            return frame;
        }
    }
}
