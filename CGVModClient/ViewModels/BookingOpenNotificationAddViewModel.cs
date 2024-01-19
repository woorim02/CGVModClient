
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

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

    public BookingOpenNotificationInfo Info { get; set; }

    public BookingOpenNotificationAddViewModel() 
    {
        targetDate = DateTime.Now.AddDays(1);
        Info = new BookingOpenNotificationInfo();
    }

    public async Task LoadAsync()
    {

    }
}
