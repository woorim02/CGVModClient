
using CommunityToolkit.Mvvm.ComponentModel;

namespace CGVModClient.ViewModels;

public partial class BookingOpenNotificationAddViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    Movie? movie;
    [ObservableProperty]
    string? movieFormat;
    [ObservableProperty]
    Theater? theater;
    [ObservableProperty]
    DateTime? targetDate;

    public BookingOpenNotificationAddViewModel() 
    {
        targetDate = DateTime.Now.AddDays(1);
    }

    public async Task LoadAsync()
    {
        
    }
}
