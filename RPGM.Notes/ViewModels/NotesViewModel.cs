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

        public NotesViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            add = new RelayCommand(OnAdd);
            delete = new RelayCommand<Guid>(OnDelete);
            rename = new RelayCommand<Guid>(OnRename);

            if (IsInDesignMode)
            {
                notes.Add(new Note { Title = "Plot ideas", DateModified = DateTimeOffset.UtcNow });
                notes.Add(new Note { Title = "May", DateModified = DateTimeOffset.UtcNow.AddMinutes(-47) });
                notes.Add(new Note { Title = "Cormac", DateModified = DateTimeOffset.UtcNow.Date.AddHours(-1) });
                notes.Add(new Note { Title = "Imps", DateModified = DateTimeOffset.UtcNow.AddDays(-10) });
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
            foreach (var note in await Database.ListAsync())
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
            notes.Remove(notes.Single(x => x.Id == id));
            await Database.DeleteAsync(id);
        }

        private void OnRename(Guid id)
        {
            Navigation.NavigateTo("Rename", id);
        }
    }
}