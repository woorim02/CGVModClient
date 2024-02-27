namespace CGVModClient.Pages;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void GoGiveawayEventListPageButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Constants.PageRoutes[typeof(GiveawayEventsPage)]);
    }

    private async void GoOpenNotificationButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Constants.PageRoutes[typeof(OpenNotificationSettingPage)]);
    }

    private async void GoAutoGiveawaySignupPageButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Constants.PageRoutes[typeof(AutoGiveawayEventSignupPage)]);
    }
}