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
        private readonly Note note;
        private readonly ICommand view;

        public NoteItemViewModel(Note note, IDatabase database, INavigationService navigation)
        {
            if (note == null) throw new ArgumentNullException("note");
            if (database == null) throw new ArgumentNullException("database");
            if (navigation == null) throw new ArgumentNullException("navigation");

            this.database = database;
            this.delete = DelegateCommand.FromAsyncHandler(Delete);
            this.navigation = navigation;
            this.note = note;
            this.view = new DelegateCommand(View);
        }

        public DateTimeOffset DateModified
        {
            get { return note.DateModified; }
        }

        public ICommand DeleteCommand
        {
            get { return delete; }
        }

        public Guid Id
        {
            get { return note.Id; }
        }

        public string Title
        {
            get { return note.Title; }
        }

        public ICommand ViewCommand
        {
            get { return view; }
        }

        public async Task Delete()
        {
            await database.DeleteAsync(note.Id);
            // TODO: event aggregator
        }

        public void View()
        {
            navigation.Navigate("Note", note.Id);
        }
    }
}
