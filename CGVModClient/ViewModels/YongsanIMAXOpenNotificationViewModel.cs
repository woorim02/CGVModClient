#if ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
using CGVModClient.Platforms.Android.Services;
#endif
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.ViewModels
{
    public partial class YongsanIMAXOpenNotificationViewModel : ObservableObject
    {
        private bool isRunning;
        private bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                if (isRunning)
                    State = "Stop";
                else
                    State = "Start";
            }
        }
        [ObservableProperty]
        private string state;
#if ANDROID
        Android.Content.Intent? intent;
#endif
        [RelayCommand]
        private void Action()
        {
#if ANDROID
            if (intent == null)
                intent = new Android.Content.Intent(Android.App.Application.Context, typeof(MyForegroundService));
            if (!IsRunning){
                Android.App.Application.Context.StartForegroundService(intent);
                IsRunning = true;
            }
            else{
                Android.App.Application.Context.StopService(intent);
                IsRunning = false;
            }
#endif
        }

        public void Load()
        {
#if ANDROID
            ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);
            var services = manager.GetRunningServices(int.MaxValue);
            foreach (var service in services)
            {
                if (service.Service.ClassName.Contains(nameof(MyForegroundService)))
                {
                    IsRunning = true;
                    return;
                }
            }
            IsRunning = false;
#endif
        }
    }
}
