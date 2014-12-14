using System;
using GalaSoft.MvvmLight;

namespace RPGM.Notes.Models
{
    public class Note : ObservableObject
    {
        private DateTimeOffset dateCreated;
        private Guid id;
        private string title;

        public DateTimeOffset DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; } // TODO: Should be getter only
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Title
        {
            get { return title; }
            set
            {
                Set<string>(ref title, value);
            }
        }

        public static Note New(string title = null)
        {
            return new Note
                {
                    dateCreated = DateTimeOffset.UtcNow,
                    id = Guid.NewGuid(),
                    title = title
                };
        }
    }
}
