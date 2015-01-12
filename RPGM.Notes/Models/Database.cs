﻿using System;
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
                await DeleteAsync<Note>(id).ConfigureAwait(false);
            }
        }

        public Task<Note> GetAsync(Guid id)
        {
            // We want to return null if not found, as InvalidOperationException is too nondescriptive
            return FindAsync<Note>(id);
        }

        public async Task<IEnumerable<Note>> ListAsync()
        {
            // TODO: Find a Linq way to exclude RtfContent (and other unnecessary properties)
            // TODO: Consider direct SQL until above
            // NOTE: This internally throws and is handled by core DLL, but still shows in output
            return await Table<Note>().OrderByDescending(x => x.DateModified).ToListAsync()/*.ConfigureAwait(false)*/;
        }

        public Task SaveAsync(Note note)
        {
            // Set/update dates
            var now = DateTimeOffset.UtcNow;
            if (note.Id == Guid.Empty) note.DateCreated = now;
            note.DateModified = now;

            return InsertOrReplaceAsync(note);
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
