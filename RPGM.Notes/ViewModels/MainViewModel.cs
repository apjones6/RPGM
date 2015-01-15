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

        private IList<Note> selectedItems;
        private bool selectMode;

        public MainViewModel(INavigationService navigation, IDatabase database)
            : base(navigation, database)
        {
        }

        public bool CanDeleteSelected
        {
            get { return selectedItems != null && selectedItems.Count > 0; }
        }

        public bool IsNotSelectMode
        {
            get { return !selectMode; }
        }

        public bool IsSelectMode
        {
            get { return selectMode; }
            set
            {
                selectMode = value;
                NotifyOfPropertyChange(() => CanDeleteSelected);
                NotifyOfPropertyChange(() => IsNotSelectMode);
                NotifyOfPropertyChange(() => IsSelectMode);
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
                IsSelectMode = selectedItems.Count > 0;

                NotifyOfPropertyChange(() => CanDeleteSelected);
                NotifyOfPropertyChange(() => SelectedItems);
            }
        }

        private void BackPressed(object sender, BackPressedEventArgs e)
        {
            if (IsSelectMode)
            {
                IsSelectMode = false;
                e.Handled = true;
            }
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
            IsSelectMode = false;

            await Database.DeleteAsync(ids);
        }

        public void New()
        {
            Navigation.NavigateToViewModel<NoteViewModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            HardwareButtons.BackPressed += BackPressed;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            HardwareButtons.BackPressed -= BackPressed;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();
            foreach (var note in await Database.ListAsync())
            {
                notes.Add(note);
            }
        }

        public void Select()
        {
            IsSelectMode = true;
        }

        public void View(Note note)
        {
            Navigation
                .UriFor<NoteViewModel>()
                .WithParam(x => x.Id, note.Id)
                .Navigate();
        }
    }
}