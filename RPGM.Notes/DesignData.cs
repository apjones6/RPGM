using System;
using System.Collections.Generic;

namespace RPGM.Notes.DesignData
{
#if DEBUG
    public class NoteList
    {
        public NoteList()
        {
            Notes = new List<object>
                {
                    new Note { Title = "Monster: Fire Imp", DateModified = DateTimeOffset.UtcNow.Subtract(new TimeSpan(0, 13, 0)) },
                    new Note { Title = "Wizard", DateModified = DateTimeOffset.UtcNow.Subtract(new TimeSpan(2, 0, 0)) },
                    new Note { Title = "Encounter 4: The Lost Shipwreck Cove", DateModified = DateTimeOffset.UtcNow.Subtract(new TimeSpan(2, 0, 0, 0)) },
                    new Note { Title = "Cheat Sheet", DateModified = DateTimeOffset.UtcNow.Subtract(new TimeSpan(9, 0, 0, 0)) }
                };
        }

        public List<object> Notes { get; set; }
    }

    public class Note
    {
        public Note()
        {
            Title = "Wizard";
        }

        public DateTimeOffset DateModified { get; set; }
        public string Title { get; set; }
    }
#endif
}
