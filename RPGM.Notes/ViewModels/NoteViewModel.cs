using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Messages;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private readonly ICommand save;

        private Note note;
        private int i;

        public NoteViewModel(INavigationService navigation)
            : base(navigation)
        {
            this.save = new RelayCommand(OnSave);
        }

        public ICommand SaveCommand
        {
            get { return save; }
        }

        public override void Initialize(object parameter)
        {
            if (parameter == null)
            {
                note = null;
            }
            else if (parameter is Guid)
            {
                // TODO: Load the appropriate note from persistence layer
                note = null;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void OnSave()
        {
            if (note == null)
            {
                note = Note.New(string.Format("Note {0}", i++));
            }

            // TODO: Save the note (asynchronous?)

            MessengerInstance.Send(new NoteMessage(note), NoteMessage.TOKEN_SAVE);
            
            // TODO: Navigate to note contents
            Navigation.GoBack();
        }
    }
}
