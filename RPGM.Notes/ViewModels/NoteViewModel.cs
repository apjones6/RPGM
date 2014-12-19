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

        private Note note;

        public NoteViewModel(INavigationService navigation)
            : base(navigation)
        {
            delete = new RelayCommand(OnDelete, () => IsEdit);
            save = new RelayCommand(OnSave);

            if (IsInDesignMode)
            {
                note = new Note { Title = "Plot ideas" };
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
            get { return note != null ? note.Title : null; }
            set
            {
                if (note != null)
                {
                    note.Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public override async Task Initialize(object parameter)
        {
            if (parameter != null)
            {
                // TODO: Try/catch
                note = await Database.Current.GetAsync<Note>(parameter);
                RaisePropertyChanged("Title");
            }
            else
            {
                note = Note.New();
            }
        }

        private async void OnDelete()
        {
            await Database.Current.DeleteAsync(note);

            // TODO: Navigate forward to notes list, and possibly clean back stack
            Navigation.GoBack();
        }

        private async void OnSave()
        {
            await Database.Current.InsertOrReplaceAsync(note);

            // TODO: Navigate to note contents (sometimes?)
            Navigation.GoBack();
        }
    }
}
