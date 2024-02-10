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
            var list = await database.Table<OpenNotificationInfo>().ToListAsync();
            foreach (var item in list)
            {
                var movie = await database.Table<Movie>()
                    .Where(x => x.Index == item.MovieIndex)
                    .FirstOrDefaultAsync();
                var theater = await database.Table<Theater>()
                    .Where(x => x.TheaterCode == item.TheaterCode)
                    .FirstAsync();
                item.Movie = movie;
                item.Theater = theater;
            }
            return list;
        }

        public async Task SaveOpenNotificationInfoAsync(OpenNotificationInfo info)
        {
            await Init();
            await database.InsertAsync(info);
            if (info.Movie != null)
                await database.InsertOrReplaceAsync(info.Movie);
            await database.InsertOrReplaceAsync(info.Theater);
        }

        public async Task DeleteOpenNotificationInfo(OpenNotificationInfo info)
        {
            await Init();
            await database.DeleteAsync(info);
        }
    }
}
