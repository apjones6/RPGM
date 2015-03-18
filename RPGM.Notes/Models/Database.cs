using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;

namespace RPGM.Notes.Models
{
    public interface IDatabase
    {
        Task DeleteAsync(Guid id);
        Task DeleteAsync(IEnumerable<Guid> ids);
        Task<Note> GetAsync(Guid id);
        Task<IEnumerable<Note>> ListAsync();
        Task SaveAsync(Note note);
    }

    [Export(typeof(IDatabase))]
    [Shared]
    public sealed class Database : SQLiteAsyncConnection, IDatabase
    {
        private static readonly Lazy<RPGMConnection> connection = new Lazy<RPGMConnection>(() => new RPGMConnection());
        private readonly IDictionary<Guid, Note> cache = new Dictionary<Guid, Note>();

        private bool isInitialized;

        public Database()
            : base(() => connection.Value)
        {
        }

        public async Task DeleteAsync(Guid id)
        {
            await DeleteAsync<Note>(id).ConfigureAwait(false);
            cache.Remove(id);
        }

        public async Task DeleteAsync(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                await DeleteAsync<Note>(id).ConfigureAwait(false);
                cache.Remove(id);
            }
        }

        public async Task<Note> GetAsync(Guid id)
        {
            return isInitialized ? (cache.ContainsKey(id) ? cache[id] : null) : await FindAsync<Note>(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Note>> ListAsync()
        {
            // TODO: Find a Linq way to exclude RtfContent (and other unnecessary properties)
            // TODO: Consider direct SQL until above
            if (!isInitialized)
            {
                isInitialized = true;
                foreach (var note in await Table<Note>().ToListAsync().ConfigureAwait(false))
                {
                    cache.Add(note.Id, note);
                }
            }

            return cache
                .Values
                .OrderByDescending(x => x.DateModified)
                .ToArray();
        }

        public async Task SaveAsync(Note note)
        {
            // Set/update dates
            var now = DateTimeOffset.UtcNow;
            if (note.Id == Guid.Empty) note.DateCreated = now;
            note.DateModified = now;

            await InsertOrReplaceAsync(note).ConfigureAwait(false);
            cache[note.Id] = note;
        }

        private class RPGMConnection : SQLiteConnectionWithLock
        {
            public RPGMConnection()
                : base(new SQLitePlatformWinRT(), new SQLiteConnectionString("rpgm.db", false))
            {
                CreateTable<Note>();
            }
        }
    }
}
