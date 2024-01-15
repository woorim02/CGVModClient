using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CGVModClient.ViewModels;

public class BookingOpenNotificationSettingViewModel : INotifyPropertyChanged, IViewModel
{
    CgvService service = new CgvService();

    public bool IsOpenNotificationEnabled {
        get => Preferences.Get("IsOpenNotificationEnabled", false);
        set { 
            Preferences.Set("IsOpenNotificationEnabled", value);
            OnPropertyChanged(nameof(IsOpenNotificationEnabled));
        }
    }

    public async Task LoadAsync()
    {
        
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
