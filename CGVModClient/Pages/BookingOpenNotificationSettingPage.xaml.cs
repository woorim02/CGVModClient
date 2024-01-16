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

    private async void AddNotificationButton_Clicked(object sender, EventArgs e)
    {
        var popup = new BookingOpenNotificationAddPage();
        await AppShell.Current.GoToAsync("BookingOpenNotificationAddPage");
    }
}