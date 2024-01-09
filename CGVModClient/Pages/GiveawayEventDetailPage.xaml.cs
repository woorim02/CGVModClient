namespace CGVModClient.Pages;

[QueryProperty(nameof(EventIndex), "eventIndex")]
public partial class GiveawayEventDetailPage : ContentPage
{
	string eventIndex;
	GiveawayEventDetailViewModel ViewModel { get; set; }
    public string EventIndex { get => eventIndex; set => eventIndex = value; }

    public GiveawayEventDetailPage() {
        InitializeComponent();
        ViewModel = new GiveawayEventDetailViewModel();
        BindingContext = ViewModel;
    }

	public GiveawayEventDetailPage(string eventIndex)
	{
		this.eventIndex = eventIndex;
        InitializeComponent();
		ViewModel = new GiveawayEventDetailViewModel();
		BindingContext = ViewModel;
	}

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await ViewModel.LoadAsync(eventIndex);
        Title = ViewModel.Title;
    }
}