using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CGVModClient.ViewModels;

public class OpenNotificationSettingViewModel : ObservableObject, IViewModel
{
    private bool IsOpenNotificationEnabled {
        get => Preferences.Get("IsOpenNotificationEnabled", false);
        set { 
            Preferences.Set("IsOpenNotificationEnabled", value);
            OnPropertyChanged(nameof(IsOpenNotificationEnabled));
        }
    }

    public ObservableCollection<OpenNotificationInfo> Infos { get; set; }

    public async Task LoadAsync()
    {
        
    }
}
