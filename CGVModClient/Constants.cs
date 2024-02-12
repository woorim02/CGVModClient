namespace CGVModClient;

public static class Constants
{
    public const string AppDatabaseFilename = "cgvmodclient-appdatabase.db";

    public const SQLite.SQLiteOpenFlags AppDatabaseFlags = 
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    public static string AppDatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, AppDatabaseFilename);

    public static IReadOnlyDictionary<Type, string> PageRoutes { get; private set; }

    static Constants()
    {
        PageRoutes = new Dictionary<Type, string>() {
            {typeof(GiveawayEventListPage), "GiveawayEvent" },
            {typeof(GiveawayEventDetailPage), "GiveawayEvent/Detail" },
            {typeof(OpenNotificationSettingPage), "OpenNotification" },
            {typeof(OpenNotificationAddPage), "OpenNotification/Add" },
        };
    }
}
