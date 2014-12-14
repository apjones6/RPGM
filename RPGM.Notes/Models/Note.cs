using System;
using System.Runtime.Serialization;

namespace RPGM.Notes.Models
{
    [DataContract]
    public class Note
    {
        [DataMember]
        private readonly DateTimeOffset dateCreated;
        [DataMember]
        private readonly Guid id;
        [DataMember]
        private string title;
        
        public Note()
        {
            dateCreated = DateTimeOffset.UtcNow;
            id = Guid.NewGuid();
        }

        public DateTimeOffset DateCreated
        {
            get { return dateCreated; }
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
    }
}
