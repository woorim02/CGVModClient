using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Database
{
    public class AppDatabase
    {
        SQLiteAsyncConnection database;
        SQLiteConnection connection;
        public AppDatabase()
        {
        }

        async Task Init()
        {
            if (database is not null)
                return;
            database = new SQLiteAsyncConnection(Constants.AppDatabasePath, Constants.AppDatabaseFlags);
            connection = new SQLiteConnection(Constants.AppDatabasePath, Constants.AppDatabaseFlags);
            await database.CreateTableAsync<Movie>();
            await database.CreateTableAsync<Theater>();
            var result = await database.CreateTableAsync<OpenNotificationInfo>();
        }

        public async Task<List<OpenNotificationInfo>> GetOpenNotificationInfosAsync()
        {
            await Init();
            return connection.GetAllWithChildren<OpenNotificationInfo>();
        }

        public async Task SaveOpenNotificationInfoAsync(OpenNotificationInfo info)
        {
            await Init();
            await database.InsertAsync(info);
        }

        public async Task InsertOrReplaceAsync<T>(T item)
        {
            await database.InsertOrReplaceAsync(item);
        }
    }
}
