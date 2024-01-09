using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CGVModClient.ViewModels;

public class GiveawayEventDetailViewModel : INotifyPropertyChanged
{
    CGVEventService eventService = new CGVEventService();
    private string title;
    private string description;
    private GiveawayEventModel eventModel;
    private GiveawayTheaterInfo theaterInfo;

    public string Title { get => title; set { 
            title = value;
            OnPropertyChanged(nameof(Title));
        } }
    public string Description { get => description; set { 
            description = value;
            OnPropertyChanged(nameof(Description));
        } }
    public GiveawayEventModel EventModel { get => eventModel; set {
            eventModel = value;
            OnPropertyChanged(nameof(EventModel));
        } }
    public GiveawayTheaterInfo TheaterInfo { get => theaterInfo; set {
            theaterInfo = value;
            OnPropertyChanged(nameof(TheaterInfo));
        } }

    public async Task GetTheaterInfoAsync(string areacode)
    {
        TheaterInfo = await eventService.GetGiveawayTheaterInfoAsync(EventModel.GiveawayIndex, int.Parse(areacode));
    }

    public async Task LoadAsync(string eventIndex)
    {
        EventModel = await eventService.GetGiveawayEventModelAsync(eventIndex);
        var task = eventService.GetGiveawayTheaterInfoAsync(EventModel.GiveawayIndex);
        var delayTask = Task.Delay(300);
        Title = EventModel.Title;
        description = EventModel.Contents;
        await delayTask;
        theaterInfo = await task;
        OnPropertyChanged(nameof(TheaterInfo));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
