namespace CGVModClient.Pages;

[QueryProperty(nameof(EventIndex), "eventIndex")]
public partial class GiveawayEventDetailPage : ContentPage
{
    private GiveawayEventDetailViewModel viewModel;
    public string EventIndex { get; set; }

    public GiveawayEventDetailPage() {
        InitializeComponent();
        viewModel = new GiveawayEventDetailViewModel();
        BindingContext = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await Task.Delay(300);
        await viewModel.LoadAsync(EventIndex);
    }
}