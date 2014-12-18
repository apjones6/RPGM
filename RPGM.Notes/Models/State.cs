using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace RPGM.Notes.Models
{
    public class State
    {
        private static State current;

        private readonly NoteCollection notes;

        private State()
        {
            notes = new NoteCollection();
        }

        public static State Current
        {
            get { return current ?? (current = new State()); }
        }

        public NoteCollection Notes
        {
            get { return notes; }
        }

        public Task InitializeAsync()
        {
            using (var database = new SQLiteConnection(new SQLitePlatformWinRT(), "notes.db"))
            {
                // TODO: Ensure indexes as application requires
                database.CreateTable<Note>();
            }

            return Task.FromResult(0);
        }

        public Task LoadAsync()
        {
            return Task.FromResult(0);
        }

        public async Task SaveAsync()
        {
            await notes.SaveAsync();
        }
    }
}
