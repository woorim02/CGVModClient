using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Platforms.Android.Services;

[Service]
public class MyForegroundService : Service
{
    private string Foreground_CHANNEL_ID = "1000";
    private int Foreground_ID = 1;
    private string Foreground_CHANNEL_NAME = "notification";

    private string OPEN_CHANNEL_ID = "2000";
    private int OPEN_ID = 2;
    private string OPEN_CHANNEL_NAME = "open_notification";

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        var foregroundChannel = new NotificationChannel(Foreground_CHANNEL_ID, Foreground_CHANNEL_NAME, NotificationImportance.Low); 
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            notificationManager.CreateNotificationChannel(foregroundChannel);

        var openChannel = new NotificationChannel(OPEN_CHANNEL_ID, OPEN_CHANNEL_NAME, NotificationImportance.High);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            notificationManager.CreateNotificationChannel(openChannel);
        var notification = new NotificationCompat.Builder(this, Foreground_CHANNEL_ID)
            .SetAutoCancel(false)
            .SetOngoing(true)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle("용아맥 오픈알림")
            .SetContentText("용아맥 오픈 확인중");
        StartForeground(Foreground_ID, notification.Build());

        _cts = new CancellationTokenSource();
        _isRunning = true;
        Task.Run(() => RunTask(_cts.Token));

        return StartCommandResult.Sticky;
    }

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    private async Task RunTask(CancellationToken cts)
    {
        while (_isRunning && !cts.IsCancellationRequested)
        {
            // 예매 스케줄을 API로 가져오는 작업 수행
            // 새로운 스케줄이 있다면 푸시 알림을 보내는 로직 추가

            // 1분마다 작업을 반복하도록 설정
            SendOpenNotification("adsfsadf", "fsad");
            await Task.Delay(TimeSpan.FromMinutes(1), cts);
        }
    }

    private void SendOpenNotification(string title, string message)
    {
        var notificationManager = NotificationManagerCompat.From(this);

        var notification = new NotificationCompat.Builder(this, OPEN_CHANNEL_ID)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetAutoCancel(true);

        notificationManager.Notify(OPEN_ID, notification.Build());
    }
}
