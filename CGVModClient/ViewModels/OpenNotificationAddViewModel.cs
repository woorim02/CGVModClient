
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace CGVModClient.ViewModels;

public partial class OpenNotificationAddViewModel : ObservableObject
{
    [ObservableProperty]
    Movie? movie;
    [ObservableProperty]
    string? movieFormat;
    [ObservableProperty]
    Theater? theater;
    [ObservableProperty]
    DateTime? targetDate;

    public OpenNotificationInfo Info { get; set; }

    public OpenNotificationAddViewModel() 
    {
        targetDate = DateTime.Now.AddDays(1);
        Info = new OpenNotificationInfo();
    }

    public void Load()
    {

    }
}
