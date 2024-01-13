namespace CGVModClient.Pages;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void GoGiveawayEventListPageButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"GiveawayEventListPage");
    }

    private void GoOpenNotificationButton_Clicked(object sender, EventArgs e)
    {

    }
}