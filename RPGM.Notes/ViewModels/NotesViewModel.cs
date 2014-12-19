using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public class NotesViewModel : ViewModel
    {
        private readonly ICommand add;
        private readonly ICommand delete;
        private readonly ObservableCollection<Note> notes = new ObservableCollection<Note>();
        private readonly ICommand rename;

        public NotesViewModel(INavigationService navigation)
            : base(navigation)
        {
            add = new RelayCommand(OnAdd);
            delete = new RelayCommand<Guid>(OnDelete);
            rename = new RelayCommand<Guid>(OnRename);

            if (IsInDesignMode)
            {
                notes.Add(new Note { Title = "Plot ideas", DateCreated = DateTimeOffset.UtcNow });
                notes.Add(new Note { Title = "May", DateCreated = DateTimeOffset.UtcNow.AddMinutes(-47) });
                notes.Add(new Note { Title = "Cormac", DateCreated = DateTimeOffset.UtcNow.Date.AddHours(-1) });
                notes.Add(new Note { Title = "Imps", DateCreated = DateTimeOffset.UtcNow.AddDays(-10) });
            }
        }

        public ICommand AddCommand
        {
            get { return add; }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
        }

        public ICommand RenameCommand
        {
            get { return rename; }
        }

        public override async Task Initialize(object parameter)
        {
            notes.Clear();
            foreach (var note in await Database.Current.Table<Note>().OrderByDescending(x => x.DateCreated).ToListAsync())
            {
                notes.Add(note);
            }
        }

        private void OnAdd()
        {
            // NOTE: The rename page will create a note as it hasn't an id
            Navigation.NavigateTo("Rename");
        }

        private async void OnDelete(Guid id)
        {
            await Database.Current.DeleteAsync<Note>(id);
            notes.Remove(notes.Single(x => x.Id == id));
        }

        private void OnRename(Guid id)
        {
            Navigation.NavigateTo("Rename", id);
        }
    }
}