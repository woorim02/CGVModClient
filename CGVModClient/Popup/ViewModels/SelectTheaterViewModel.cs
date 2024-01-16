using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CGVModClient.Popup.ViewModels;

public class SelectTheaterViewModel : INotifyPropertyChanged, IViewModel
{
    CgvService service = new CgvService();

    List<Area> areas;
    List<Theater> theaters;

    public List<Area> Areas { 
        get => areas;
        set {
            areas = value;
            OnPropertyChanged(nameof(Areas));
        }
    }
    public List<Theater> Theaters {
        get => theaters;
        set { 
            theaters = value;
            OnPropertyChanged($"{nameof(Theaters)}");
        }
    }
    public Command<string> SelectAreaCommand {  get; set; }
    public Command SelectTheaterCommand { get; set; }

    public SelectTheaterViewModel()
    {
        SelectAreaCommand = new Command<string>(async (s) =>
        {
            Theaters = new List<Theater>(await service.GetTheatersAsync(s));
        });
    }

    public async Task LoadAsync()
    {
        Areas = new List<Area>( await service.GetAreasAsync());
        Theaters = new List<Theater>(await service.GetTheatersAsync(Areas[0].AreaCode));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
