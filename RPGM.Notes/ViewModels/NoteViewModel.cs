using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public class NoteViewModel : ViewModel
    {
        private readonly ICommand delete;
        private readonly ICommand save;

        private Note note;
        private string title;

        public NoteViewModel(INavigationService navigation)
            : base(navigation)
        {
            delete = new RelayCommand(OnDelete, () => IsEdit);
            save = new RelayCommand(OnSave);

            if (IsInDesignMode)
            {
                title = "Plot ideas";
            }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public ICommand SaveCommand
        {
            get { return save; }
        }

        public bool IsEdit
        {
            get { return note != null && note.Id != Guid.Empty; }
        }

        public string Title
        {
            get { return title; }
            set { Set<string>(ref title, value, "Title"); }
        }

        public override Task Initialize(object parameter)
        {
            if (parameter == null)
            {
                note = new Note { DateCreated = DateTimeOffset.UtcNow };
            }
            else
            {
                note = State.Notes.FirstOrDefault(x => parameter.Equals(x.Id));
            }

            // NOTE: Perhaps be robust here and just default to new
            if (note == null)
            {
                throw new ArgumentException("No note could be found for provided parameter.", "parameter");
            }
            
            // Copy properties so we don't update on cancel
            title = note.Title;

            return Task.FromResult(0);
        }

        private void OnDelete()
        {
            // TODO: Delete the note (asynchronous?)
            State.Notes.Remove(note);

            // TODO: Navigate to notes list, and possibly clean back stack
            Navigation.GoBack();
        }

        private void OnSave()
        {
            note.Title = title;

            // TODO: Save the note (asynchronous?)
            if (note.Id == Guid.Empty)
            {
                State.Notes.Insert(0, note);
            }

            // TODO: Navigate to note contents (sometimes?)
            Navigation.GoBack();
        }
    }
}
