using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ICommand delete;
        private readonly RelayCommand deleteSelection;
        private readonly ObservableCollection<Note> notes = new ObservableCollection<Note>();
        private readonly ICommand rename;
        private readonly ISet<Guid> selectedIds = new HashSet<Guid>();
        private readonly ICommand selectionChanged;
        private readonly ICommand select;
        private readonly ICommand tap;

        private bool selectable;

        public NotesViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            Messenger.Register<BackMessage>(this, OnBackMessage);

            add = new RelayCommand(OnAdd);
            delete = new RelayCommand<Guid>(OnDelete);
            deleteSelection = new RelayCommand(OnDeleteSelection, () => selectedIds.Count > 0);
            rename = new RelayCommand<Guid>(OnRename);
            selectionChanged = new RelayCommand<IList<object>>(OnSelectionChanged);
            select = new RelayCommand(OnSelect);
            tap = new RelayCommand<Guid>(OnTap);

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

        public ICommand DeleteSelectionCommand
        {
            get { return deleteSelection; }
        }

        public bool IsSelectable
        {
            get { return selectable; }
            set
            {
                if (selectable != value)
                {
                    selectable = value;
                    RaisePropertyChanged("IsSelectable");
                }
            }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
        }

        public ICommand RenameCommand
        {
            get { return rename; }
        }

        public ICommand SelectionChangedCommand
        {
            get { return selectionChanged; }
        }

        public ICommand SelectCommand
        {
            get { return select; }
        }

        public ICommand TapCommand
        {
            get { return tap; }
        }

        public override async Task InitializeAsync(object parameter)
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

        private void OnBackMessage(BackMessage message)
        {
            if (IsSelectable)
            {
                message.Handled = true;
                IsSelectable = false;
            }
        }

        private async void OnDelete(Guid id)
        {
            notes.Remove(notes.Single(x => x.Id == id));
            await Database.DeleteAsync(id);
        }

        private async void OnDeleteSelection()
        {
            var ids = selectedIds.ToArray();
            foreach (var id in ids)
            {
                notes.Remove(notes.Single(x => x.Id == id));
            }

            IsSelectable = false;
            selectedIds.Clear();

            await Database.DeleteAsync(ids);
        }

        private void OnRename(Guid id)
        {
            Navigation.NavigateTo("Rename", id);
        }

        private void OnSelectionChanged(IList<object> items)
        {
            selectedIds.Clear();
            foreach (Note note in items)
            {
                selectedIds.Add(note.Id);
            }

            // Deselect all items cancels multiple selection mode
            deleteSelection.RaiseCanExecuteChanged();
            IsSelectable = selectedIds.Any();
        }

        private void OnSelect()
        {
            deleteSelection.RaiseCanExecuteChanged();
            IsSelectable = true;
        }

        private void OnTap(Guid id)
        {
            Navigation.NavigateTo("Note", id);
        }
    }
}