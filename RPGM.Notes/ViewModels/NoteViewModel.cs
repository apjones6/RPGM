using System;
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

        private bool isNew;
        private Note note;
        private string title;

        public NoteViewModel(INavigationService navigation)
            : base(navigation)
        {
            delete = new RelayCommand(OnDelete, () => !isNew);
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
            get { return !isNew; }
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
                note = new Note();
                isNew = true;
            }
            else if (parameter is Guid)
            {
                note = State.Notes[(Guid)parameter];
                isNew = false;
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
            if (isNew)
            {
                State.Notes.Add(note);
            }

            // TODO: Navigate to note contents (sometimes?)
            Navigation.GoBack();
        }
    }
}
