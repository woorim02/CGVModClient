using CommunityToolkit.Maui.Views;

namespace CGVModClient.Pages;

public partial class BookingOpenNotificationAddPage : ContentPage
{
	public BookingOpenNotificationAddPage()
	{
		InitializeComponent();
	}

    private async void TitleButton_Clicked(object sender, EventArgs e)
    {

    }
    private void MovieFormatButton_Clicked(object sender, EventArgs e)
    {

    }

    private async void TheaterButton_Clicked(object sender, EventArgs e)
    {
        var popup = new SelectTheaterPopup();
        var res = await this.ShowPopupAsync(popup);
    }

    private void DateButton_Clicked(object sender, EventArgs e)
    {

    }

    private void ConfirmButton_Clicked(object sender, EventArgs e)
    {

    }
}