using CGVModClient.ViewModels;

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

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        
    }
}