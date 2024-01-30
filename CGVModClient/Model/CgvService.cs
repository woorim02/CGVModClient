using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CGVModClient.Model;

public class CgvService
{
    private static HttpClient defaultClient;
    private static HttpClient authClient;
    private static SocketsHttpHandler authHandler;

    private static Aes _aes;
    private static SHA256 _sha256;
    private static MD5 _md5;

    static CgvService()
    {
        defaultClient = new HttpClient(new SocketsHttpHandler()
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        });
        defaultClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        defaultClient.DefaultRequestHeaders.Host = "m.cgv.co.kr";

        authHandler = new SocketsHttpHandler()
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };
        authClient = new HttpClient(authHandler);
        authClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        authClient.DefaultRequestHeaders.Host = "m.cgv.co.kr";

        _aes = Aes.Create();
        _aes.IV = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNA==");
        _aes.Key = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNDc1YzM5YzY1ZGQwZTFlOTQ=");
        _sha256 = SHA256.Create();
        _md5 = MD5.Create();
    }

    public CgvEventService Event { get; private set; }
    public CgvAuthService Auth { get; private set; }

    public CgvService()
    {
        Event = new CgvEventService(defaultClient, _aes);
        Auth = new CgvAuthService(authClient, authHandler, _aes, _sha256, _md5);
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
        for (int i = 0; i < areasNode.Count; i++)
        {
            try
            {
                var regioncode = areasNode[i].SelectSingleNode("a").Attributes["href"].Value.Split('=')[1];
                var innerText = areasNode[i].SelectSingleNode("a/div/strong").InnerText.Replace(")", string.Empty);
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
            catch (Exception ex)
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
        return theaters?.ToArray() ?? new Theater[0];
    }

    public async Task<Movie[]> GetMoviesAsync()
    {
        var movies = new List<Movie>();
        var firstRequest = new HttpRequestMessage(HttpMethod.Get, "https://m.cgv.co.kr/WebAPP/MovieV4/movieList.aspx?iPage=1");
        var firstResponse = await defaultClient.SendAsync(firstRequest);
        firstResponse.EnsureSuccessStatusCode();
        var firstContent = await firstResponse.Content.ReadAsStringAsync();
        var firstList = ParseMovieList(firstContent);
        movies.AddRange(firstList);
        if (firstList.Count < 20)
            return movies.ToArray();
        for (int i = 2; true; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebAPP/MovieV4/ajaxMovie.aspx");
            var payload = new Dictionary<string, string>
            {
                { "iPage" , $"{i}"},
                { "pageRow" , $"{20}"},
                { "mtype", "now" },
                { "morder", "TicketRate" },
                { "mnowflag",  $"{0}" },
                { "mdistype", "" },
                { "flag", "MLIST" }
            };
            request.Content = new FormUrlEncodedContent(payload);
            var response = await defaultClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var list = ParseMovieList(content);
            movies.AddRange(list);
            if (list.Count < 20)
                break;
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
        for (int i = 0; i < nodes.Count; i++)
        {
            var title = nodes[i].Attributes["alt"].Value;
            var imgSource = nodes[i].Attributes["src"].Value;
            var sp = imgSource.Split('/');
            var index = imgSource.Split('/')[sp.Length - 1].Split('_')[0];
            var movie = new Movie()
            {
                Title = title,
                ThumbnailSource = imgSource,
                Index = index,

            };
            movies.Add(movie);
        }
        return movies.ToArray();
    }

    public async Task<string> GetMovieGroupCdAsync(string movieIndex)
    {
        var htmlText = await GetFanpageHtmlText(movieIndex);
        var reader = new StringReader(htmlText);
        while (true)
        {
            string? line = reader.ReadLine();
            if (line == null)
                throw new InvalidDataException("무비그룹을 찾을 수 없음");
            if (line.Contains("mgCD"))
            {
                var value = Regex.Replace(line, @"\D", string.Empty);
                return value;
            }
        }
    }

    public async Task<string[]> GetScreenTypesAsync(string movieIndex)
    {
        var list = new List<string> { "2D" };
        var document = new HtmlDocument();
        var text = await GetFanpageHtmlText(movieIndex);
        if (text.Contains("페이지를 찾을 수 없습니다."))
            throw new InvalidDataException("페이지를 찾을 수 없습니다.");
        document.LoadHtml(text);
        var nodes = document.DocumentNode.SelectNodes("//ul[@class='screenType']/li/img");
        if (nodes == null)
            return list.ToArray();
        foreach (var n in nodes)
            list.Add(n.Attributes["alt"].Value);
        return list.ToArray();
    }

    public async Task<TheaterScheduleListRoot> GetScheduleListAsync(string theaterCode, DateTime date, string screenTypeCode = "02")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://m.cgv.co.kr/WebApp/Reservation/Schedule.aspx");
        await defaultClient.SendAsync(request);

        request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebAPP/Reservation/Common/ajaxTheaterScheduleList.aspx/GetTheaterScheduleList");
        request.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
        request.Headers.Add("Accept-Language", "ko-KR,ko;q=0.8,en-US;q=0.5,en;q=0.3");
        request.Headers.Add("Origin", "https://m.cgv.co.kr");
        request.Headers.Add("Cookie", "URL_PREV_COMMON=");
        var body = new
        {
            strRequestType = "THEATER",
            strUserID = "",
            strMovieGroupCd = "",
            strMovieTypeCd = "",
            strPlayYMD = $"{date:yyyyMMdd}",
            strTheaterCd = theaterCode,
            strScreenTypeCd = screenTypeCode,
            strRankType = "MOVIE",
        };
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        var response = await defaultClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        content = JObject.Parse(content)["d"].ToString();
        var root = JsonConvert.DeserializeObject<TheaterScheduleListRoot>(content);
        return root;
    }

    private async Task<string> GetFanpageHtmlText(string movieIndex)
    {
        var gateWayResponse = await defaultClient.GetAsync($"https://m.cgv.co.kr/WebApp/fanpage/Gateway.aspx?movieIdx={movieIndex}");
        var request = new HttpRequestMessage(HttpMethod.Post, $"https://moviestory.cgv.co.kr/fanpage/login");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        request.Headers.Host = "moviestory.cgv.co.kr";
        request.Headers.Add("Cookie", gateWayResponse.Headers.GetValues("Set-Cookie"));
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"fanpageMovieIdx", movieIndex },
            {"fanpageIsWebView","false" }
        });
        //request.Headers.Host = "moviestory.cgv.co.kr";
        var response = await defaultClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    private static List<Movie> ParseMovieList(string content)
    {
        List<Movie> movies = new List<Movie>();
        var document = new HtmlDocument();
        document.LoadHtml(content);
        var imageNodes = document.DocumentNode.SelectNodes("//span[@class='imgbox']/img");
        var contentsNodes = document.DocumentNode.SelectNodes("//div[@class='txtbox']");
        var jsNodes = document.DocumentNode.SelectNodes("//a[@class='btn_reserve']");
        var icoTheaterNodes = document.DocumentNode.SelectNodes("//div[@class='ico_theater2']");
        if ((imageNodes.Count != contentsNodes.Count) || (contentsNodes.Count != jsNodes.Count) || (jsNodes.Count != icoTheaterNodes.Count))
            throw new InvalidDataException($"Count err! {imageNodes.Count}, {contentsNodes.Count}, {jsNodes.Count}, {icoTheaterNodes.Count}");
        if (imageNodes.Count == 0)
            return movies;
        for (int i = 0; i < contentsNodes.Count; i++)
        {
            try
            {
                string title = imageNodes[i].Attributes["alt"].Value;
                string imgSource = imageNodes[i].Attributes["src"].Value;

                var spArr = imgSource.Split("/");
                string index = imgSource.Split('/')[spArr.Length - 1].Split('_')[0];
                string movieGroupCd = jsNodes[i].Attributes["onclick"].Value.Split("', '")[1];

                var spanNodes = icoTheaterNodes[i].SelectNodes("span");
                List<string> screenTypes = ["2D"];
                var list = spanNodes?.Select(s => s.InnerText);
                if (list != null)
                    screenTypes.AddRange(list);

                var movie = new Movie()
                {
                    Title = title,
                    Index = index,
                    ThumbnailSource = imgSource,
                    MovieGroupCd = movieGroupCd,
                    ScreenTypes = screenTypes.ToArray()
                };
                movies.Add(movie);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("MovieList Parse err!", ex);
            }
        }
        return movies;
    }
}
