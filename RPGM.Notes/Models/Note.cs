using System;
using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;

namespace RPGM.Notes.Models
{
    public class Note : ObservableObject
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
            set { Set<DateTimeOffset>(ref dateCreated, value, "DateCreated"); }
        }

        public DateTimeOffset DateModified
        {
            get { return dateModified; }
            set { Set<DateTimeOffset>(ref dateModified, value, "DateModified"); }
        }

        [AutoIncrement]
        [PrimaryKey]
        public Guid Id
        {
            get { return id; }
            set { Set<Guid>(ref id, value, "Id"); }
        }

        public string RtfContent
        {
            get { return rtfContent; }
            set { Set<string>(ref rtfContent, value, "RtfContent"); }
        }

        public string Title
        {
            get { return title; }
            set { Set<string>(ref title, value, "Title"); }
        }
    }
}
