using HtmlAgilityPack;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CGVModClient.Model;

public class CgvService
{
    private static HttpClient defaultClient;
    private static HttpClient authClient;

    static CgvService()
    {
        defaultClient = new HttpClient();
        defaultClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        defaultClient.DefaultRequestHeaders.Host = "m.cgv.co.kr";

        authClient = new HttpClient();
    }

    public CgvService() 
    { 

    }

    public async Task<Area[]> GetAreasAsync()
    {
        var response = await defaultClient.GetAsync("https://m.cgv.co.kr/WebApp/TheaterV4/");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = new HtmlDocument();
        document.LoadHtml(content);

        var areaCgvNode = document.DocumentNode.SelectSingleNode("//div[@class='cgv_choice linktype area']");
        var areasNode = areaCgvNode.SelectNodes("ul/li");
        var areas = new List<Area>();
        for(int i = 0; i< areasNode.Count; i++)
        {
            try
            {
                var regioncode = areasNode[i].SelectSingleNode("a").Attributes["href"].Value.Split('=')[1];
                var innerText = areasNode[i].SelectSingleNode("a/div/strong").InnerText.Replace(")",string.Empty);
                var name = innerText.Split("(")[0];
                var count = innerText.Split("(")[1];
                name = Regex.Replace(name, @"\s+", string.Empty);
                count = Regex.Replace(count, @"\s+", string.Empty);
                areas.Add(new Area
                {
                    AreaCode = regioncode,
                    AreaName = name,
                    TheaterCount = count
                });
            }
            catch(Exception ex)
            { 
                throw new InvalidDataException(content, ex);
            }
        }
        return areas.ToArray();
    }

    public async Task<Theater[]> GetTheatersAsync(string regionCode)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebApp/MyCgvV5/favoriteTheater.aspx/GetRegionTheaterList");
        request.Content = new StringContent($"{{ regionCode: '{regionCode}'}}", Encoding.UTF8, "application/json");
        var response = await defaultClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        List<Theater> theaters;
        try
        {
            var obj = JObject.Parse(content);
            var arr = JArray.Parse(obj["d"].ToString());
            theaters = arr.ToObject<List<Theater>>();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(content, ex);
        }
        return theaters.ToArray();
    }

    public async Task<Movie[]> GetMoviesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://www.cgv.co.kr/movies/?lt=1&ft=0");
        request.Headers.Add("Accept", "ext/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        request.Headers.Host = "www.cgv.co.kr";
        var response = await defaultClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var document = new HtmlDocument();
        document.LoadHtml(content);
        var imageNodes = document.DocumentNode.SelectNodes("//span[@class='thumb-image']");
        var contentsNodes = document.DocumentNode.SelectNodes("//div[@class='box-contents']");
        if (imageNodes.Count != contentsNodes.Count)
            throw new InvalidDataException($"imageNodes.Count({imageNodes.Count} != contentsNodes.Count({contentsNodes.Count})");
        var movies = new List<Movie>();
        for( int i = 0; i < contentsNodes.Count; i++)
        {
            try
            {
                movies.Add(new Movie() { 
                    Title = contentsNodes[i].SelectSingleNode("a/strong").InnerText, 
                    Index = contentsNodes[i].SelectSingleNode("a").Attributes["href"].Value.Split('=')[1],
                    ThumbnailSource = imageNodes[i].SelectSingleNode("img").Attributes["src"].Value,
                });
            }
            catch (Exception ex) { 
                throw new InvalidDataException(content, ex);
            }
        }
        return movies.ToArray();
    }

    public async Task<Movie[]?> GetMoviesAsync(string keyword)
    {
        var checkRequest = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebAPP/Search/Default.aspx/CheckKeyword");
        checkRequest.Content = new StringContent($"{{keyword: '{HttpUtility.UrlEncode(keyword)}'}}", Encoding.UTF8, "application/json");
        var checkResponse = await defaultClient.SendAsync(checkRequest);
        checkResponse.EnsureSuccessStatusCode();
        var checkResponseObject = JObject.Parse(await checkResponse.Content.ReadAsStringAsync());
        if (checkResponseObject["d"]?.ToString() != "00000")
            return null;

        var getMovieListRequest = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebAPP/Search/Default.aspx/GetMovieInfoList");
        getMovieListRequest.Content = new StringContent($"{{ pageIndex: '1', pageSize:'30', keyword: '{HttpUtility.UrlEncode(keyword)}'}}", Encoding.UTF8, "application/json");
        var getMovieListResponse = await defaultClient.SendAsync(getMovieListRequest);
        getMovieListResponse.EnsureSuccessStatusCode();
        var obj = JObject.Parse(await getMovieListResponse.Content.ReadAsStringAsync());

        string? countValue = obj?["d"]?["Count"]?.ToString();
        if (string.IsNullOrEmpty(countValue) || countValue == "0")
            return null;

        var document = new HtmlDocument();
        document.LoadHtml(obj["d"]["Contents"].ToString());
        var nodes = document.DocumentNode.SelectNodes("//img");
        var movies = new List<Movie>();
        for(int i = 0; i<nodes.Count; i++)
        {
            var movie = new Movie() {
                Title = nodes[i].Attributes["alt"].Value,
                ThumbnailSource = nodes[i].Attributes["src"].Value
            };
            var sp = movie.ThumbnailSource.Split('/');
            movie.Index = movie.ThumbnailSource.Split('/')[sp.Length - 1].Split('_')[0];
            movies.Add(movie);
        }
        return movies.ToArray();
    }
}
