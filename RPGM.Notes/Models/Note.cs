using System;
using Caliburn.Micro;
using SQLite.Net.Attributes;

namespace RPGM.Notes.Models
{
    public class Note : PropertyChangedBase
    {
        private DateTimeOffset dateCreated;
        private DateTimeOffset dateModified;
        private Guid id;
        private string rtfContent;
        private string title;

        public Note()
        {
        }

        public Note(Note source)
        {
            if (source == null) throw new ArgumentNullException("source");

            this.dateCreated = source.dateCreated;
            this.dateModified = source.dateModified;
            this.id = source.id;
            this.rtfContent = source.rtfContent;
            this.title = source.title;
        }

        public DateTimeOffset DateCreated
        {
            get { return dateCreated; }
            set
            {
                dateCreated = value;
                NotifyOfPropertyChange(() => DateCreated);
            }
        }

        public DateTimeOffset DateModified
        {
            get { return dateModified; }
            set
            {
                dateModified = value;
                NotifyOfPropertyChange(() => DateModified);
            }
        }

        [AutoIncrement]
        [PrimaryKey]
        public Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string RtfContent
        {
            get { return rtfContent; }
            set
            {
                rtfContent = value;
                NotifyOfPropertyChange(() => RtfContent);
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && Id == ((Note)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
