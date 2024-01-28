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
    private string OPEN_GROUP_KEY = "OPEN_GROUP_KEY";
    private string OPEN_GROUP_NAME = "OPEN_GROUP_NAME";

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
        notificationManager.CreateNotificationChannelGroup(new NotificationChannelGroup(OPEN_GROUP_KEY, OPEN_GROUP_NAME));
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

    private async Task RunTask(CancellationToken cts)
    {
        var service = new CgvService();
        TheaterScheduleListRoot root;
        string[] days;
        root = await service.GetScheduleListAsync("0013", DateTime.Now.AddDays(1));
        days = root.ResultSchedule.ListPlayYmd.Split('|');
        while (_isRunning && !cts.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts);
            var nowRoot = await service.GetScheduleListAsync("0013", DateTime.Now.AddDays(1));
            var nowDays = nowRoot.ResultSchedule.ListPlayYmd.Split('|');
            var diff = nowDays.Where(x =>
            {
                var a = days.Contains(x);
                return !a;
            }).ToList();
            if (diff.Count() != 0) {
                if (diff[0] == days[0] && diff.Count == 1){
                    //다음날로 넘어가서 당일 스케쥴이 사라진 경우
                    days = nowDays;
                    continue;
                }
                if (diff[0] == days[0] && diff.Count != 1)
                {
                    //다음날로 넘어감과 동시에 오픈된 경우
                    diff.RemoveAt(0);
                }
                     
                SendOpenNotification("용아맥 오픈", $"용아맥 오픈 {string.Join(", ", diff)}일");
                days = nowDays;
            }
        }
    }

    private void SendOpenNotification(string title, string message)
    {
        var notificationManager = NotificationManagerCompat.From(this);

        var notification = new NotificationCompat.Builder(this, OPEN_CHANNEL_ID)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetGroup(OPEN_GROUP_KEY);
            //.SetPriority((int)NotificationImportance.High);

        notificationManager.Notify(OPEN_ID, notification.Build());
    }
}
