using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Messages;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public class NotesViewModel : ViewModel
    {
        private readonly ICommand add;
        private readonly ObservableCollection<Note> notes;

        public NotesViewModel(INavigationService navigation)
            : base(navigation)
        {
            MessengerInstance.Register<NoteMessage>(this, NoteMessage.TOKEN_SAVE, OnNoteMessage);

            this.add = new RelayCommand(OnAdd);
            this.notes = new ObservableCollection<Note>();

            if (IsInDesignMode)
            {
                notes.Add(new Note { DateCreated = DateTimeOffset.UtcNow.AddHours(-127), Title = "Plot ideas" });
                notes.Add(new Note { DateCreated = DateTimeOffset.UtcNow.AddHours(-52), Title = "Cormac" });
                notes.Add(new Note { DateCreated = DateTimeOffset.UtcNow.AddHours(-9), Title = "Imps" });
            }
        }

        public ICommand AddCommand
        {
            get { return add; }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
        }

        private void OnAdd()
        {
            // NOTE: The rename page will create a note as it hasn't an id
            Navigation.NavigateTo("Rename");
        }

        private void OnNoteMessage(NoteMessage message)
        {
            if (!notes.Any(x => x.Id == message.Note.Id))
            {
                notes.Add(message.Note);
            }
        }
    }
}