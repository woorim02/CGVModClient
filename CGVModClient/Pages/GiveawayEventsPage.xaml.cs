namespace CGVModClient.Pages;

public partial class GiveawayEventsPage : ContentPage
{
    GiveawayEventsViewModel viewModel;

    public GiveawayEventsPage()
	{
		InitializeComponent();
		viewModel = new GiveawayEventsViewModel();
        BindingContext = viewModel;
	}

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await viewModel.LoadAsync();
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var args = (GiveawayEvent)e.Item;
        await Shell.Current.GoToAsync($"{Constants.PageRoutes[typeof(GiveawayEventDetailPage)]}?eventIndex={args.EventIndex}");
    }
}