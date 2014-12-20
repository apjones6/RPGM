using System;
using System.Collections.Generic;
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

    public sealed class Database : SQLiteAsyncConnection, IDatabase
    {
        private static readonly Lazy<RPGMConnection> connection = new Lazy<RPGMConnection>(() => new RPGMConnection());

        public Database()
            : base(() => connection.Value)
        {
        }

        public Task DeleteAsync(Guid id)
        {
            return DeleteAsync<Note>(id);
        }

        public async Task DeleteAsync(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                await DeleteAsync<Note>(id);
            }
        }

        public Task<Note> GetAsync(Guid id)
        {
            return GetAsync<Note>(id);
        }

        public async Task<IEnumerable<Note>> ListAsync()
        {
            return await Table<Note>().OrderByDescending(x => x.DateModified).ToListAsync();
        }

        public async Task SaveAsync(Note note)
        {
            // Set/update dates
            var now = DateTimeOffset.UtcNow;
            if (note.Id == Guid.Empty) note.DateCreated = now;
            note.DateModified = now;

            await InsertOrReplaceAsync(note);
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
