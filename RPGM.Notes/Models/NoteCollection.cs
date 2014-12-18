using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace RPGM.Notes.Models
{
    public class NoteCollection : ObservableCollection<Note>
    {
        private readonly ISet<Guid> delete;
        private readonly DataContractJsonSerializer serializer;
        private bool loaded;

        public NoteCollection()
        {
            this.delete = new HashSet<Guid>();
            this.serializer = new DataContractJsonSerializer(typeof(Note[]));
        }

        public bool IsLoaded
        {
            get { return loaded; }
        }

        public Task LoadAsync()
        {
            if (loaded)
            {
                return Task.FromResult(0);
            }

            using (var database = new SQLiteConnection(new SQLitePlatformWinRT(), "notes.db"))
            {
                foreach (var note in database.Table<Note>())
                {
                    Add(note);
                }
            }

            loaded = true;
            return Task.FromResult(0);
        }

        public void Remove(Guid id)
        {
            var note = this.FirstOrDefault(x => x.Id == id);
            if (note == null)
            {
                throw new ArgumentException("No item in collection with provided id.", "id");
            }

            Remove(note);
        }

        public Task SaveAsync()
        {
            using (var database = new SQLiteConnection(new SQLitePlatformWinRT(), "notes.db"))
            {
                // TODO: Only save dirty notes
                foreach (var note in this)
                {
                    database.InsertOrReplace(note, typeof(Note));
                }

                // Delete notes
                foreach (var id in delete)
                {
                    database.Delete<Note>(id);
                }

                // No need to remember these IDs now
                delete.Clear();
            }

            return Task.FromResult(0);
        }

        protected override void RemoveItem(int index)
        {
            // We'll need this to remove from database
            var note = this[index];
            if (note != null && note.Id != Guid.Empty)
            {
                delete.Add(note.Id);
            }

            base.RemoveItem(index);
        }
    }
}
