namespace CGVModClient.Pages;

public partial class BookingOpenNotificationSettingPage : ContentPage
{
    BookingOpenNotificationSettingViewModel viewModel;
    public BookingOpenNotificationSettingPage()
    {
        InitializeComponent();
        viewModel = new BookingOpenNotificationSettingViewModel();
        BindingContext = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await viewModel.LoadAsync();
    }

    private void AddNotificationButton_Clicked(object sender, EventArgs e)
    {
        File.WriteAllText("C:\\Users\\woorim\\source\\vscode\\ttt.txt", Newtonsoft.Json.Linq.JObject.FromObject(Preferences.Default).ToString());
    }
}