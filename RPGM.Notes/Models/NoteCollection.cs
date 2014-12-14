using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RPGM.Notes.Models
{
    public class NoteCollection : ObservableCollection<Note>
    {
        private readonly DataContractJsonSerializer serializer;
        private bool loaded;

        public NoteCollection()
        {
            this.serializer = new DataContractJsonSerializer(typeof(Note[]));
        }

        public bool IsLoaded
        {
            get { return loaded; }
        }

        public Note this[Guid id]
        {
            get { return this.FirstOrDefault(x => x.Id == id); }
        }

        public async Task LoadAsync()
        {
            if (loaded)
            {
                return;
            }

            StorageFile file;
            try
            {
                file = await ApplicationData.Current.RoamingFolder.GetFileAsync("notes.json");
            }
            catch (FileNotFoundException)
            {
                loaded = true;
                return;
            }

            var text = await FileIO.ReadTextAsync(file);
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(text));
            var notes = (Note[])serializer.ReadObject(stream);
            
            foreach (var note in notes)
            {
                Add(note);
            }

            loaded = true;
        }

        public async Task SaveAsync()
        {
            // TODO: Only save when dirty
            var notes = this.ToArray();
            var stream = new MemoryStream();
            serializer.WriteObject(stream, notes);

            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("notes.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(file, stream.ToArray());
        }
    }
}
