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
        await AppShell.Current.GoToAsync("BookingOpenNotificationAddPage");
    }
}