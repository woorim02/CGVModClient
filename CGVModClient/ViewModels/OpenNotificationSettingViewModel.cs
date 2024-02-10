#if ANDROID
using Android.App;
using Android.Content;
using CGVModClient.Platforms.Android.Services;
#endif
using CommunityToolkit.Mvvm.ComponentModel;

namespace CGVModClient.ViewModels;

public partial class OpenNotificationSettingViewModel : ObservableObject, IViewModel
{
    private AppDatabase _database;

    [ObservableProperty]
    private List<OpenNotificationInfo> infos;
    public bool IsOpenNotificationEnabled {
        get => Preferences.Get("IsOpenNotificationEnabled", false);
        set { 
            Preferences.Set("IsOpenNotificationEnabled", value);
            OnPropertyChanged(nameof(IsOpenNotificationEnabled));
        }
    }

    public OpenNotificationSettingViewModel(AppDatabase database)
    {
        _database = database;
    }

    public async Task LoadAsync()
    {
#if ANDROID
        ActivityManager manager = (ActivityManager)Android.App.Application.Context.GetSystemService(Context.ActivityService);
        var services = manager.GetRunningServices(int.MaxValue);
        foreach (var service in services)
        {
            if (service.Service.ClassName.Contains(nameof(MyForegroundService)))
            {
                IsOpenNotificationEnabled = true;
                break;
            }
        }
#endif
        Infos = await _database.GetOpenNotificationInfosAsync();
    }

    public async Task RemoveOpenNotificationInfo(OpenNotificationInfo info)
    {
        Infos.Remove(info);
        OnPropertyChanged(nameof(Infos));
        await _database.DeleteOpenNotificationInfo(info);
    }
}
