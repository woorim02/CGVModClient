using CGVModClient.Popup.ViewModels;
using CommunityToolkit.Maui.Views;

namespace CGVModClient.Popup;

public partial class SelectTheaterPopup : CommunityToolkit.Maui.Views.Popup
{
	SelectTheaterViewModel viewModel;

	public SelectTheaterPopup()
	{
		InitializeComponent();
		viewModel = new SelectTheaterViewModel();
		BindingContext = viewModel;
	}

    private async void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
		await viewModel.LoadAsync();
    }

    private void TheaterButton_Clicked(object sender, EventArgs e)
    {
		var button = sender as Button;
		CloseAsync(button?.CommandParameter);
    }
}