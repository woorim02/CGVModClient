#if ANDROID
using Android.App;
using Android.Content;
using CGVModClient.Platforms.Android.Services;
#endif
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CGVModClient.ViewModels;

public partial class OpenNotificationSettingViewModel : ObservableObject, IViewModel
{
    private AppDatabase _database;
    public ObservableCollection<OpenNotificationInfo> Infos { get; private set; }
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
        Infos = new ObservableCollection<OpenNotificationInfo>();
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
        var infos = await _database.GetOpenNotificationInfosAsync();
        var comparer = EqualityComparer<OpenNotificationInfo>.Create((x, y) => x?.Id == y?.Id);
        for (var i = 0; i < infos.Count; i++)
        {
            if (!Infos.Contains(infos[i], comparer))
                Infos.Add(infos[i]);
        }
    }

    [RelayCommand]
    public async Task RemoveOpenNotificationInfo(OpenNotificationInfo info)
    {
        var result = await Microsoft.Maui.Controls.Application.Current.MainPage
            .DisplayAlert("예매 오픈 알림 설정", "알림을 삭제하시겠습니까?", "삭제", "취소", FlowDirection.LeftToRight);
        if (!result)
            return;
        Infos.Remove(info);
        await _database.DeleteOpenNotificationInfo(info);
    }
}
