using CGVModClient.Data;
using CGVModClient.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CGVModClient.ViewModels;

public partial class GiveawayEventListViewModel : ObservableObject
{
    CgvService service = new CgvService();
    [ObservableProperty]
    private bool isBusy;
    [ObservableProperty]
    private string eventState;
    public ObservableCollection<GiveawayEvent> GiveawayEvents { get; set;}

    public async Task LoadAsync()
    {
        IsBusy = true;
        var arr = await service.Event.GetGiveawayEventsAsync();
        GiveawayEvents = new ObservableCollection<GiveawayEvent>(arr);
        SetEventState(GiveawayEvents.Count);
        OnPropertyChanged(nameof(GiveawayEvents));
        IsBusy = false;
    }

    public void SetEventState(int count)
    {
        EventState = $"진행중인 경품 이벤트 {count}개";
    }
}
