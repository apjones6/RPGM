using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using RPGM.Notes.Models;
using Windows.Phone.UI.Input;

namespace RPGM.Notes.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly ObservableCollection<Note> notes = new ObservableCollection<Note>();

        private bool selectable;
        private IList<Note> selectedItems;

        public MainViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
        }

        public bool CanDeleteSelected
        {
            get { return selectedItems != null && selectedItems.Count > 0; }
        }

        public bool IsSelectable
        {
            get { return selectable; }
            set
            {
                if (selectable != value)
                {
                    selectable = value;
                    NotifyOfPropertyChange(() => CanDeleteSelected);
                    NotifyOfPropertyChange(() => IsSelectable);

                    if (value)
                    {
                        Navigation.BackPressed += BackPressed;
                    }
                    else
                    {
                        Navigation.BackPressed -= BackPressed;
                    }
                }
            }
        }

        public ObservableCollection<Note> Notes
        {
            get { return notes; }
        }

        public object SelectedItems
        {
            get { return selectedItems; }
            set
            {
                // Only works with object property
                selectedItems = ((IList<object>)value).Cast<Note>().ToArray();

                // Deselect all items cancels multiple selection mode
                IsSelectable = selectedItems.Count > 0;

                NotifyOfPropertyChange(() => CanDeleteSelected);
                NotifyOfPropertyChange(() => SelectedItems);
            }
        }

        public void Add()
        {
            Navigation.NavigateToViewModel<NoteViewModel>();
        }

        private void BackPressed(object sender, BackPressedEventArgs e)
        {
            // Setting this false removes the handler
            IsSelectable = false;
            e.Handled = true;
        }

        public async void Delete(Note note)
        {
            await Database.DeleteAsync(note.Id);
            notes.Remove(note);
        }

        public async void DeleteSelected()
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

        protected override async void OnInitialize()
        {
            foreach (var note in await Database.ListAsync())
            {
                notes.Add(note);
            }
        }

        public void Select()
        {
            IsSelectable = true;
        }

        public void View(Note note)
        {
            Navigation.NavigateToViewModel<NoteViewModel>(note.Id);
        }
    }
}