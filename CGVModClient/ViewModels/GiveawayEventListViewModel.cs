using CGVModClient.Data;
using CGVModClient.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CGVModClient.ViewModels;

public class GiveawayEventListViewModel : INotifyPropertyChanged
{
    CgvService service = new CgvService();

    public string EventState { get; set; }
    public List<GiveawayEvent> GiveawayEventList { get; set;}

    public async Task LoadAsync()
    {
        var arr = await service.Event.GetGiveawayEventsAsync();
        GiveawayEventList = new List<GiveawayEvent>(arr);
        SetEventState(GiveawayEventList.Count);
        OnPropertyChanged(nameof(GiveawayEventList));
        OnPropertyChanged(nameof(EventState));
    }

    public void SetEventState(int count)
    {
        EventState = $"진행중인 경품 이벤트 {count}개";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
