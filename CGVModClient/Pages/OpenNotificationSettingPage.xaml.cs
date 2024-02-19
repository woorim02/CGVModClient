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
        try
        {
            await viewModel.LoadAsync();
        }
        catch(Exception ex)
        {
            File.WriteAllText("storage/emulated/0/Download/k.txt", ex.ToString());
        }
    }

    private async void AddNotificationButton_Clicked(object sender, EventArgs e)
    {
        await AppShell.Current.GoToAsync($"{Constants.PageRoutes[typeof(OpenNotificationAddPage)]}");
    }
    /*
    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var info = (OpenNotificationInfo)e.Item;
        var result = await DisplayAlert(Title, "알림을 삭제하시겠습니까?", "삭제", "취소", FlowDirection.LeftToRight);
        if(result)
        {
            await viewModel.RemoveOpenNotificationInfo(info);
        }
    }
    */
}