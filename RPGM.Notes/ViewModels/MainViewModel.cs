using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using RPGM.Notes.Models;
using Windows.UI.Xaml.Navigation;

namespace RPGM.Notes.ViewModels
{
    [Export]
    public class MainViewModel : ViewModel, IBackNavigationAware
    {
        private readonly IDatabase database;
        private readonly DelegateCommandBase deleteSelected;
        private readonly INavigationService navigation;
        private readonly ICommand @new;
        private readonly ObservableCollection<NoteItemViewModel> notes = new ObservableCollection<NoteItemViewModel>();
        private readonly ICommand select;

        private IList<NoteItemViewModel> selectedItems;
        private bool selectMode;

        [ImportingConstructor]
        public MainViewModel(IDatabase database, INavigationService navigation)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (navigation == null) throw new ArgumentNullException("navigation");

            this.database = database;
            this.deleteSelected = DelegateCommand.FromAsyncHandler(DeleteSelected, () => selectedItems != null && selectedItems.Count > 0);
            this.@new = new DelegateCommand(New);
            this.navigation = navigation;
            this.select = new DelegateCommand(Select);
        }

        public ICommand DeleteSelectedCommand
        {
            get { return deleteSelected; }
        }

        public bool IsSelectMode
        {
            get { return selectMode; }
            set
            {
                SetProperty(ref selectMode, value);
                deleteSelected.RaiseCanExecuteChanged();
            }
        }

        public ICommand NewCommand
        {
            get { return @new; }
        }

        public ObservableCollection<NoteItemViewModel> Notes
        {
            get { return notes; }
        }

        public ICommand SelectCommand
        {
            get { return select; }
        }

        public object SelectedItems
        {
            get { return selectedItems; }
            set
            {
                // Only works with object property
                SetProperty(ref selectedItems, ((IList<object>)value).Cast<NoteItemViewModel>().ToArray());

                // Deselect all items cancels multiple selection mode
                IsSelectMode = selectedItems.Count > 0;
            }
        }

        public async Task DeleteSelected()
        {
            var ids = selectedItems.Select(x => x.Id).ToArray();
            foreach (var item in selectedItems)
            {
                notes.Remove(item);
            }

            // This triggers UI to empty SelectedItems property
            IsSelectMode = false;

            await database.DeleteAsync(ids);
        }

        public void New()
        {
            navigation.Navigate("Note", null);
        }

        public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            var i = 0;
            foreach (var note in await database.ListAsync())
            {
                // Create or update item
                var item = this.notes.FirstOrDefault(x => x.Id == note.Id);
                if (item == null)
                {
                    item = new NoteItemViewModel(note, database, navigation);
                    notes.Insert(i, item);
                }
                else
                {
                    item.DateModified = note.DateModified;
                    item.Title = note.Title;

                    // Move if necessary
                    var index = notes.IndexOf(item);
                    if (index != i)
                    {
                        notes.Move(index, i);
                    }
                }

                i++;
            }

            // Remove any other items in the collection
            while (notes.Count > i)
            {
                notes.RemoveAt(i);
            }
        }

        public void Select()
        {
            IsSelectMode = true;
        }

        public bool TryGoBack()
        {
            if (IsSelectMode)
            {
                IsSelectMode = false;
                return false;
            }

            return true;
        }
    }
}