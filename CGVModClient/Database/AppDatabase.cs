using SQLite;
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

        public AppDatabase()
        {
        }

        async Task Init()
        {
            if (database is not null)
                return;
            database = new SQLiteAsyncConnection(Constants.AppDatabasePath, Constants.AppDatabaseFlags);
            await database.CreateTableAsync<Movie>();
            await database.CreateTableAsync<Theater>();
            var result = await database.CreateTableAsync<OpenNotificationInfo>();
        }

        public async Task<List<OpenNotificationInfo>> GetOpenNotificationInfosAsync()
        {
            await Init();
            return await database.Table<OpenNotificationInfo>().ToListAsync();
        }

        public async Task SaveOpenNotificationInfoAsync(OpenNotificationInfo info)
        {
            await Init();
            await database.InsertAsync(info);
        }
    }
}
