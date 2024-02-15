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

    public ObservableCollection<Theater> Theaters { get; private set; } = new ObservableCollection<Theater>();
    
    public async Task LoadAsync(string eventIndex)
    {
        IsBusy = true;
        var eventModel = await service.Event.GetGiveawayEventModelAsync(eventIndex);
        Title = eventModel.Title;
        Description = eventModel.Contents;
        var theaterInfo = await service.Event.GetGiveawayTheaterInfoAsync(eventModel.GiveawayIndex);
        foreach(var item in theaterInfo.TheaterList) {
            Theaters.Add(item);
        }
        IsBusy = false;
    }
}
