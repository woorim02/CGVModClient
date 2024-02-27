using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace CGVModClient.Platforms.Android.Services;

[Service]
public class OpenNotificationForegroundService : Service
{
    #region Fields
    private string FOREGROUND_TITLE = "예매 오픈 알림";
    private int FOREGROUND_NOTIFICATION_ID = 0;
    private int OPEN_NOTIFICATION_ID = 0;

    private CgvService _service = new CgvService();
    private AppDatabase _database = new AppDatabase();
    private CancellationTokenSource _cts;
    private bool _isRunning;
    #endregion

    #region Override Methods
    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var notification = new NotificationCompat.Builder(this, Constants.FOREGROUND_CHANNEL_ID)
            .SetAutoCancel(false)
            .SetOngoing(true)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(FOREGROUND_TITLE)
            .SetContentText("용아맥 오픈 확인중");
        StartForeground(FOREGROUND_NOTIFICATION_ID, notification.Build());

        _cts = new CancellationTokenSource();
        _isRunning = true;

        Task.Run(async () => {
            while (_isRunning && !_cts.IsCancellationRequested)
            {
                try
                {
                    await CheckOpen(_cts.Token);
                }
                catch (TaskCanceledException) { }
                catch (HttpRequestException ex)
                {
                    SendOpenNotification("오류 발생", $"{ex.Message}");
                }
                catch (Exception e)
                {
                    SendOpenNotification("알 수 없는 오류 발생", $"{e.Message}");
                    StopSelf();
                    return;
                }
            }
        });

        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        _cts.Cancel();
        _isRunning = false;
        base.OnDestroy();
    }

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }
    #endregion

    private async Task CheckOpen(CancellationToken token)
    {
        var infos = await _database.GetOpenNotificationInfosAsync();
        foreach (var info in infos)
        {
            if (info.IsOpen && info.CanReservation)
                break;
            var theaterCode = info.TheaterCode.PadLeft(4, '0');
            var movieGroupcd = info?.Movie?.MovieGroupCd;
            var targetDate = info.TargetDate;
            var screenTypeCode = GetScreenTypeCode(info.ScreenType);
            var list = await _service.Reservation.GetScheduleListAsync(theaterCode, movieGroupcd, targetDate, screenTypeCode);

            var targetScheduleList = list.ResultSchedule.ScheduleList
                .Where(x => info.Movie == null ? true : x.MovieIdx == info.MovieIndex)
                .ToList();

            if (targetScheduleList.Count == 0)
                break;

            if (!info.IsOpen)
            {
                var formattedScheduleTextList = list.ResultSchedule.ScheduleList
                    .Select(x => $"[{(x.MovieNmKor.Length > 5 ? x.MovieNmKor.Substring(0, 3) + ".." : x.MovieNmKor)}|{x.PlayStartTm.Insert(2, ":")}~]")
                    .ToList();
                
                SendOpenNotification(
                    $"{info.Theater.TheaterName} {info.TargetDate:dd}일 예매준비중",
                    $"{string.Join(", ", formattedScheduleTextList)}");

                info.IsOpen = true;
                await _database.SaveOpenNotificationInfoAsync(info);
                break;
            }
            if (!info.CanReservation)
            {
                var formattedScheduleTextList = list.ResultSchedule.ScheduleList
                    .Select(x => $"[{(x.MovieNmKor.Length > 5 ? x.MovieNmKor.Substring(0, 3) + ".." : x.MovieNmKor)}|{x.PlayStartTm.Insert(2, ":")}~]")
                    .ToList();

                SendOpenNotification(
                    $"{info.Theater.TheaterName} {info.TargetDate:dd}일 예매 오픈",
                    $"{string.Join(", ", formattedScheduleTextList)}");
                info.CanReservation = true;
                await _database.SaveOpenNotificationInfoAsync(info);
                break;
            }
        }
    }

    #region Notification Methods

    private void SetForegroundNotificationMessage(string message)
    {
        var notification = new NotificationCompat.Builder(this, Constants.FOREGROUND_CHANNEL_ID)
            .SetAutoCancel(false)
            .SetOngoing(true)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(FOREGROUND_TITLE)
            .SetContentText(message);
        var notificationManager = NotificationManagerCompat.From(this);
        notificationManager.Notify(FOREGROUND_NOTIFICATION_ID, notification.Build());
    }

    private void SendOpenNotification(string title, string message)
    {
        var notificationManager = NotificationManagerCompat.From(this);

        var notification = new NotificationCompat.Builder(this, Constants.OPEN_CHANNEL_ID)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetGroup(Constants.OPEN_GROUP_KEY);

        var groupNoti = new NotificationCompat.Builder(this, Constants.OPEN_CHANNEL_ID)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetGroup(Constants.OPEN_GROUP_KEY)
            .SetGroupSummary(true);

        notificationManager.Notify(OPEN_NOTIFICATION_ID++, notification.Build());
        notificationManager.Notify(0, groupNoti.Build());
    }
    #endregion

    #region Static Methods
    /// <summary>
    /// IMAX 이외에는 미구현 상태임
    /// </summary>
    /// <param name="screenType">스크린타입(2D, IMAX, 4DX....등)</param>
    /// <returns>strScreenTypeCd</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static string GetScreenTypeCode(string screenType)
    {
        switch (screenType.ToUpper())
        {
            case "IMAX":
                return "02";
            default:
                throw new NotImplementedException();
        }
    }
    #endregion
}
