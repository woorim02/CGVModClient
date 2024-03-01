namespace CGVModClient.Pages;

public partial class AutoGiveawayEventSignupPage : ContentPage
{
	AutoGiveawayEventSignupViewModel viewModel;
	public AutoGiveawayEventSignupPage()
	{
		BindingContext = viewModel = new AutoGiveawayEventSignupViewModel();
		InitializeComponent();
	}
}