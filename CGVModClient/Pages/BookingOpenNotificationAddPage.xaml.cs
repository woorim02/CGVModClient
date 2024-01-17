using CommunityToolkit.Maui.Views;

namespace CGVModClient.Pages;

public partial class BookingOpenNotificationAddPage : ContentPage
{
    BookingOpenNotificationAddViewModel viewModel;
	public BookingOpenNotificationAddPage()
	{
		InitializeComponent();
        viewModel = new BookingOpenNotificationAddViewModel();
        BindingContext = viewModel;
        datePicker.MinimumDate = DateTime.Now;
        datePicker.MaximumDate = DateTime.Now.AddMonths(1);
	}

    private async void TitleButton_Clicked(object sender, EventArgs e)
    {
        var popup = new SelectMoviePopup();
        Movie? movie = await this.ShowPopupAsync(popup) as Movie;
        if (movie != null)
        {
            viewModel.Movie = movie;
        }
    }
    private void MovieFormatButton_Clicked(object sender, EventArgs e)
    {

    }

    private async void TheaterButton_Clicked(object sender, EventArgs e)
    {
        var popup = new SelectTheaterPopup();
        Theater? theater = await this.ShowPopupAsync(popup) as Theater;
        if (theater != null)
        {
            viewModel.Theater = theater;
        }
    }

    private void ConfirmButton_Clicked(object sender, EventArgs e)
    {

    }
}