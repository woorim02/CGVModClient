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
    private string eventState = "진행중인 경품 이벤트 _개";
    public ObservableCollection<GiveawayEvent> GiveawayEvents { get; private set; } = new ObservableCollection<GiveawayEvent>();

    public async Task LoadAsync()
    {
        IsBusy = true;
        var arr = await service.Event.GetGiveawayEventsAsync();
        foreach (var item in arr) {
            GiveawayEvents.Add(item);
        }
        SetEventState(GiveawayEvents.Count);
        IsBusy = false;
    }

    public void SetEventState(int count)
    {
        EventState = $"진행중인 경품 이벤트 {count}개";
    }
}
