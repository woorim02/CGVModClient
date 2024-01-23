namespace CGVModClient.Pages;

public partial class YongsanIMAXOpenNotificationPage : ContentPage
{
	YongsanIMAXOpenNotificationViewModel viewModel;
	public YongsanIMAXOpenNotificationPage()
	{
		InitializeComponent();
		viewModel = new YongsanIMAXOpenNotificationViewModel();
		BindingContext = viewModel;
	}

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
		viewModel.Load();
    }
}