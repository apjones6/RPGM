using System;
using RPGM.Notes.Models;

namespace RPGM.Notes.Messages
{
    public class NoteMessage
    {
        private readonly Note note;

        public const int TOKEN_SAVE = 0;

        public NoteMessage(Note note)
        {
            if (note == null) throw new ArgumentNullException("note");
            this.note = note;
        }

        public Note Note
        {
            get { return note; }
        }
    }
}
