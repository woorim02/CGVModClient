using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CGVModClient.ViewModels;

public partial class GiveawayEventDetailViewModel : ObservableObject
{
    private CgvService service = new CgvService();

    [ObservableProperty]
    private bool isBusy = false;
    [ObservableProperty]
    private string title;
    [ObservableProperty]
    private string description;

    private GiveawayTheaterInfo theaterInfo;
    private GiveawayEventModel eventModel;

    public ObservableCollection<Theater> Theaters { get; set; }

    public async Task LoadAsync(string eventIndex)
    {
        IsBusy = true;
        eventModel = await service.Event.GetGiveawayEventModelAsync(eventIndex);
        Title = eventModel.Title;
        Description = eventModel.Contents;
        theaterInfo = await service.Event.GetGiveawayTheaterInfoAsync(eventModel.GiveawayIndex);
        Theaters = new ObservableCollection<Theater>(theaterInfo.TheaterList);
        OnPropertyChanged(nameof(Theaters));
        IsBusy = false;
    }
}
