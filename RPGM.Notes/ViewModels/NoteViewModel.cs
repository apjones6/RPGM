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
        private readonly RelayCommand save;

        private Note note;

        public NoteViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            delete = new RelayCommand(OnDelete, () => IsEdit);
            save = new RelayCommand(OnSave, CanSave);

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
                    save.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanSave()
        {
            return note != null && !string.IsNullOrWhiteSpace(note.Title);
        }

        public override async Task Initialize(object parameter)
        {
            if (parameter is Guid)
            {
                // TODO: Try/catch
                note = await Database.GetAsync((Guid)parameter);
                RaisePropertyChanged("Title");
            }
            else
            {
                note = new Note();
            }
        }

        private async void OnDelete()
        {
            await Database.DeleteAsync(note.Id);

            // TODO: Navigate forward to notes list, and possibly clean back stack
            Navigation.GoBack();
        }

        private async void OnSave()
        {
            await Database.SaveAsync(note);

            // TODO: Navigate to note contents (sometimes?)
            Navigation.GoBack();
        }
    }
}
