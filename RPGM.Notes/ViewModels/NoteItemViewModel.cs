using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using RPGM.Notes.Models;

namespace RPGM.Notes.ViewModels
{
    public class NoteItemViewModel : ViewModel
    {
        private readonly IDatabase database;
        private readonly ICommand delete;
        private readonly INavigationService navigation;
        private readonly ICommand view;

        private DateTimeOffset dateModified;
        private string title;

        public NoteItemViewModel(Note note, IDatabase database, INavigationService navigation)
        {
            if (note == null) throw new ArgumentNullException("note");
            if (database == null) throw new ArgumentNullException("database");
            if (navigation == null) throw new ArgumentNullException("navigation");

            this.database = database;
            this.dateModified = note.DateModified;
            this.delete = DelegateCommand.FromAsyncHandler(Delete);
            this.Id = note.Id;
            this.navigation = navigation;
            this.title = note.Title;
            this.view = new DelegateCommand(View);
        }

        public DateTimeOffset DateModified
        {
            get { return dateModified; }
            set { SetProperty(ref dateModified, value); }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public Guid Id { get; private set; }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ICommand ViewCommand
        {
            get { return view; }
        }

        public async Task Delete()
        {
            await database.DeleteAsync(Id);
            // TODO: event aggregator
        }

        public void View()
        {
            navigation.Navigate("Note", Id);
        }
    }
}
