using CGVModClient.Popup.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CGVModClient.Popup;

public partial class SelectMoviePopup : CommunityToolkit.Maui.Views.Popup
{
    SelectMovieViewModel viewModel;
	public SelectMoviePopup()
	{
		InitializeComponent();
        viewModel = new SelectMovieViewModel();
        BindingContext = viewModel;
	}

    private async void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        await viewModel.LoadAsync();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var movie = e.Parameter as Movie;
        await CloseAsync(movie);
    }
}