using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CGVModClient.ViewModels;

public partial class AutoGiveawayEventSignupViewModel : ObservableObject
{
    [ObservableProperty]
    private string reservationNumber;
    [ObservableProperty]
    private string phoneNumber;
    [ObservableProperty]
    private string progressText;
    [ObservableProperty]
    private bool isRunnung = false;
    CgvService _service = new CgvService();
    [RelayCommand]
    private async Task SignupAsync()
    {
        if (IsRunnung)
            return;
        IsRunnung = true;
        //todo
    }
}
