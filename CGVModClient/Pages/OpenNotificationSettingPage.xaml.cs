namespace CGVModClient.Pages;

public partial class OpenNotificationSettingPage : ContentPage
{
    OpenNotificationSettingViewModel viewModel;

    public OpenNotificationSettingPage(AppDatabase database)
    {
        InitializeComponent();
        viewModel = new OpenNotificationSettingViewModel(database);
        BindingContext = viewModel;
    }

    private async void ContentPage_Appearing(object sender, EventArgs e)
    {
        await viewModel.LoadAsync();
    }

    private async void AddNotificationButton_Clicked(object sender, EventArgs e)
    {
        await AppShell.Current.GoToAsync($"{Constants.PageRoutes[typeof(OpenNotificationAddPage)]}");
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var info = e.Item as OpenNotificationInfo;
        var result = await Application.Current.MainPage.DisplayAlert(Title, "알림을 삭제하시겠습니까?", "삭제", "취소", FlowDirection.LeftToRight);
        if(result)
        {
            await viewModel.RemoveOpenNotificationInfo(info);
        }
    }
}