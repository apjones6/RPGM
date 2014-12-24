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
        private readonly ICommand delete;
        private readonly RelayCommand deleteSelection;
        private readonly ObservableCollection<Note> notes = new ObservableCollection<Note>();
        private readonly ICommand select;

        private bool selectable;
        private IList<Note> selectedItems;

        public NotesViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
            Messenger.Register<BackMessage>(this, OnBackMessage);

            delete = new RelayCommand<Guid>(OnDelete);
            deleteSelection = new RelayCommand(OnDeleteSelection, () => selectedItems != null && selectedItems.Count > 0);
            select = new RelayCommand(() => IsSelectable = true);

            if (IsInDesignMode)
            {
                notes.Add(new Note { Title = "Plot ideas", DateModified = DateTimeOffset.UtcNow });
                notes.Add(new Note { Title = "May", DateModified = DateTimeOffset.UtcNow.AddMinutes(-47) });
                notes.Add(new Note { Title = "Cormac", DateModified = DateTimeOffset.UtcNow.Date.AddHours(-1) });
                notes.Add(new Note { Title = "Imps", DateModified = DateTimeOffset.UtcNow.AddDays(-10) });
            }
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
                    deleteSelection.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
        }

        public object SelectedItems
        {
            set
            {
                selectedItems = ((IList<object>)value).Cast<Note>().ToArray();

                // Deselect all items cancels multiple selection mode
                IsSelectable = selectedItems.Count > 0;

                deleteSelection.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedItems");
            }
        }

        public ICommand SelectCommand
        {
            get { return select; }
        }

        public override async Task InitializeAsync(object parameter)
        {
            notes.Clear();
            foreach (var note in await Database.ListAsync())
            {
                notes.Add(note);
            }
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
            var ids = selectedItems.Select(x => x.Id).ToArray();
            foreach (var note in selectedItems)
            {
                notes.Remove(note);
            }

            // This triggers UI to empty SelectedItems property
            IsSelectable = false;

            await Database.DeleteAsync(ids);
        }
    }
}