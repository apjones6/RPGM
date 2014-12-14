using System.Threading.Tasks;

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
            // NOTE: We only need to load what we definitely need here. Other components will cause
            //       what they need to be loaded.
            return Task.FromResult(0);
        }

        public async Task SaveAsync()
        {
            await notes.SaveAsync();
        }
    }
}
