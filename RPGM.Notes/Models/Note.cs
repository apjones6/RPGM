﻿using System;
using Microsoft.Practices.Prism.Mvvm;
using SQLite.Net.Attributes;

namespace RPGM.Notes.Models
{
    public class Note : BindableBase
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
            set { SetProperty(ref dateCreated, value); }
        }

        public DateTimeOffset DateModified
        {
            get { return dateModified; }
            set { SetProperty(ref dateModified, value); }
        }

        [AutoIncrement]
        [PrimaryKey]
        public Guid Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string RtfContent
        {
            get { return rtfContent; }
            set { SetProperty(ref rtfContent, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
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
