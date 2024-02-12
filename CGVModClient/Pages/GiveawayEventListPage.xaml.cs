namespace CGVModClient.Pages;

public partial class GiveawayEventListPage : ContentPage
{
    GiveawayEventListViewModel viewModel;

    public GiveawayEventListPage()
	{
		InitializeComponent();
		viewModel = new GiveawayEventListViewModel();
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