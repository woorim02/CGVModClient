namespace CGVModClient;

public static class Constants
{
#if ANDROID
    public const string FOREGROUND_CHANNEL_ID = "openNotification.foreground";
    public const string FOREGROUND_CHANNEL_NAME = "오픈알림 활성화 알림";
    public const string OPEN_CHANNEL_ID = "openNotification.channel";
    public const string OPEN_CHANNEL_NAME = "오픈알림";
    public const string OPEN_GROUP_KEY = "openNotification.group";
    public const string OPEN_GROUP_NAME = "OpenNotification NotificationGroup";
#endif
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
            {typeof(MainPage), "Main/" },
            {typeof(GiveawayEventsPage), "Main/GiveawayEvent" },
            {typeof(GiveawayEventDetailPage), "Main/GiveawayEvent/Detail" },
            {typeof(AutoGiveawayEventSignupPage), "Main/AutoGiveawayEventSignup" },
            {typeof(OpenNotificationSettingPage), "Main/OpenNotification" },
            {typeof(OpenNotificationAddPage), "Main/OpenNotification/Add" },
        };
    }
}
