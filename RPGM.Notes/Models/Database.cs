using System;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;

namespace RPGM.Notes.Models
{
    public sealed class Database : SQLiteAsyncConnection
    {
        private static readonly Lazy<RPGMConnection> connection = new Lazy<RPGMConnection>(() => new RPGMConnection());
        private static Database current;

        public Database()
            : base(() => connection.Value)
        {
        }

        public static Database Current
        {
            get { return current ?? (current = new Database()); }
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
