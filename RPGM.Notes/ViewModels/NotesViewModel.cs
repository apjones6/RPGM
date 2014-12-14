using System;
using System.Collections.ObjectModel;
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
        private readonly ICommand rename;

        public NotesViewModel(INavigationService navigation)
            : base(navigation)
        {
            add = new RelayCommand(OnAdd);
            rename = new RelayCommand<Guid>(OnRename);

            if (IsInDesignMode)
            {
                Notes.Add(new Note { Title = "Plot ideas" });
                Notes.Add(new Note { Title = "Cormac" });
                Notes.Add(new Note { Title = "Imps" });
            }
        }

        public ICommand AddCommand
        {
            get { return add; }
        }

        public ObservableCollection<Note> Notes
        {
            get { return State.Notes; }
        }

        public ICommand RenameCommand
        {
            get { return rename; }
        }

        public override async Task Initialize(object parameter)
        {
            await State.Notes.LoadAsync();
        }

        private void OnAdd()
        {
            // NOTE: The rename page will create a note as it hasn't an id
            Navigation.NavigateTo("Rename");
        }

        private void OnRename(Guid id)
        {
            Navigation.NavigateTo("Rename", id);
        }
    }
}