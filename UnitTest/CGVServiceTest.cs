using System.Diagnostics;
using Xunit.Abstractions;

namespace UnitTest;

public class CGVServiceTest
{
    readonly CgvService service = new CgvService();

    [Fact]
    public async void GetAreasTest()
    {
        Area[] areas = await service.GetAreasAsync();
        Assert.True(areas.Length != 0);
        Assert.NotNull(areas.First().AreaName);
        Assert.NotNull(areas.First().AreaCode);
        Assert.NotNull(areas.First().TheaterCount);
    }

    [Fact]
    public async void GetTheatersTest()
    {
        Theater[] theaters = await service.GetTheatersAsync("01");
        Assert.True(theaters.Length != 0);
        Assert.NotNull(theaters.First().TheaterName);
        Assert.NotNull(theaters.First().TheaterCode);
        Assert.Null(theaters.First().EncCount);
        Assert.Null(theaters.First().CountTypeCode);
        Assert.Null(theaters.First().GiveawayRemainCount);
        Assert.Null(theaters.First().GiveawayName);
        Assert.Null(theaters.First().ReceiveTypeCode);
    }

    [Fact]
    public async void GetMoviesTest()
    {
        Movie[] movies = await service.GetMoviesAsync();
        Assert.True(movies.Length != 0);
        Assert.NotNull(movies.First().Title);
        Assert.NotNull(movies.First().Index);
        Assert.NotNull(movies.First().ThumbnailSource);
        Assert.NotNull(movies.First().MovieGroupCd);
        Assert.True(movies.First().ScreenTypes?.Length != 0);
    }

    [Fact]
    public async void GetMoviesWithKeywordTest()
    {
        Movie[]? movies = await service.GetMoviesAsync("¸í·®");
        Assert.NotNull(movies);
        Assert.NotNull(movies.First().Title);
        Assert.NotNull(movies.First().Index);
        Assert.NotNull(movies.First().ThumbnailSource);
        Assert.Null(movies.First().MovieGroupCd);
        Assert.Null(movies.First().ScreenTypes);
    }

    [Fact]
    public async void GetMoviesWithInvalidKeywordTest()
    {
        Movie[]? movies = await service.GetMoviesAsync("¯—µå¹Ö´Ù´íµð");
        Assert.Null(movies);
    }

    [Fact]
    public async void GetMovieGroupCdTest()
    {
        string mgCd = await service.GetMovieGroupCdAsync("85997");
        Assert.True(mgCd == "20029909");
    }

    [Fact]
    public async void GetMovieGroupCdWithInvalidMovieIndex()
    {
        await Assert.ThrowsAsync<InvalidDataException>(async () => await service.GetMovieGroupCdAsync("00000000"));
    }

    [Fact]
    public async void GetScreenTypesTest()
    {
        var movies = await service.GetMoviesAsync();
        var list = movies.Where(x => x.ScreenTypes?.Length > 1);
        Assert.True(list.Count() > 0);
        var types = await service.GetScreenTypesAsync(list.First().Index);
        Assert.True(list.First()?.ScreenTypes?.Length == types.Length);
    }

    [Fact]
    public async void GetScreenTypesWithInvalidIndex()
    {
        await Assert.ThrowsAsync<InvalidDataException>(async () => await service.GetScreenTypesAsync("015464654645"));
    }
}