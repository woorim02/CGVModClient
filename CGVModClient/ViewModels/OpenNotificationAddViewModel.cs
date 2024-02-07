
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    DateTime targetDate;

    AppDatabase database;

    public OpenNotificationAddViewModel(AppDatabase database) 
    {
        targetDate = DateTime.Now.AddDays(1);
        movieFormat = "IMAX";//Todo
        this.database = database;
    }

    [RelayCommand]
    private async Task Confirm()
    {
        if (Movie != null && MovieFormat != null && Theater != null)
            return;

        var info = new OpenNotificationInfo()
        {
            Movie = Movie,
            ScreenType = MovieFormat,
            Theater = Theater,
            TargetDate = TargetDate
        };
        await database.SaveOpenNotificationInfoAsync(info);
    }
}
