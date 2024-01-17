using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Popup.ViewModels;

public partial class SelectMovieViewModel : ObservableObject, IViewModel
{
    private CgvService service = new CgvService();
    [ObservableProperty]
    private List<Movie> movieList;

    public async Task LoadAsync()
    {
        MovieList = new List<Movie>(await service.GetMoviesAsync());
    }

    [RelayCommand]
    public async Task SearchMovie(string keyword)
    {
        var result = await service.GetMoviesAsync(keyword);
        if (result == null)
        {
            MovieList = new List<Movie>();
            return;
        }
        MovieList = new List<Movie>(result);
    }
}
