
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
    Action ClosePage { get; set; }

    public OpenNotificationAddViewModel(AppDatabase database, Action closePage) 
    {
        targetDate = DateTime.Now.AddDays(1);
        movieFormat = "IMAX";//Todo
        this.database = database;
        ClosePage = closePage;
    }

#pragma warning disable CS8602 // Application.Current.MainPage == null 일 가능성 배제
    [RelayCommand]
    private async Task Confirm()
    {
        if (Theater == null) {
            await Application.Current.MainPage.DisplayAlert("알림 설정", "극장을 선택해 주세요", "확인");
            return;
        }
        if (MovieFormat == null) {
            await Application.Current.MainPage.DisplayAlert("알림 설정", "영화 포맷을 선택해 주세요", "확인");
            return;
        }
#pragma warning restore CS8602

        var info = new OpenNotificationInfo()
        {
            Movie = Movie,
            ScreenType = MovieFormat,
            Theater = Theater,
            TargetDate = TargetDate
        };
        await database.SaveOpenNotificationInfoAsync(info);
        ClosePage();
        return;
    }
}
