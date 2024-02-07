using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        Infos = await _database.GetOpenNotificationInfosAsync();
    }
}
