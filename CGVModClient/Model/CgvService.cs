using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Text;

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
        request.Content = new StringContent("{ regionCode: ''}", Encoding.UTF8, "application/json");
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
}
