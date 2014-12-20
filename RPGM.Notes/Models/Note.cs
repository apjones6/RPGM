﻿using System;
using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;

namespace RPGM.Notes.Models
{
    public class Note : ObservableObject
    {
        private DateTimeOffset dateCreated;
        private DateTimeOffset dateModified;
        private Guid id;
        private string title;

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

        public string Title
        {
            get { return title; }
            set { Set<string>(ref title, value, "Title"); }
        }
    }
}