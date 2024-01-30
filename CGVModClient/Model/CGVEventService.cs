using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CGVModClient.Model;

public class CgvEventService
{
    private readonly HttpClient _client;
    private readonly Aes _aes;

    public CgvEventService(HttpClient client, Aes aes) 
    {
        _client = client;
        _aes = aes;
        _aes.IV = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNA==");
        _aes.Key = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNDc1YzM5YzY1ZGQwZTFlOTQ=");
    }

    public async Task<GiveawayEvent[]> GetGiveawayEventsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Event/GiveawayEventList.aspx/GetGiveawayEventList");
        request.Content = new StringContent("{theaterCode: '', pageNumber: '1', pageSize: '30'}", Encoding.UTF8, "application/json");
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(content);
        var document = new HtmlDocument();

        try {
            var htmlText = obj["d"]["List"].ToString().Replace(" onclick='detailEvent(this, \"False\")'", "");
            document.LoadHtml(htmlText); 
        }
        catch (Exception e){ throw new InvalidDataException(content, e); }

        List<GiveawayEvent> list = new List<GiveawayEvent>();
        try
        {
            foreach (var i in document.DocumentNode.ChildNodes)
            {
                GiveawayEvent giveawayEvent = new GiveawayEvent()
                {
                    EventIndex = i.Attributes["data-eventIdx"].Value,
                    Title = i.SelectSingleNode("div/strong[1]").InnerText,
                    Period = i.SelectSingleNode("div/span[1]").InnerText,
                    DDay = (i.SelectSingleNode("div/span[2]").InnerText)
                };
                list.Add(giveawayEvent);
            }
        }
        catch(Exception e) { throw new InvalidDataException(content, e); }
        return list.ToArray();
    }

    public async Task<GiveawayEventModel> GetGiveawayEventModelAsync(string eventIndex)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Event/GiveawayEventDetail.aspx/GetGiveawayEventDetail");
        request.Content = new StringContent($"{{eventIndex: '{eventIndex}', giveawayIndex: ''}}", Encoding.UTF8, "application/json");
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(content);
        var model = obj["d"]?.ToObject<GiveawayEventModel>();
        if(model == null) { throw new InvalidDataException(content); }

        return model;
    }

    public async Task<GiveawayTheaterInfo> GetGiveawayTheaterInfoAsync(string giveawayIndex, string areaCode = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Event/GiveawayEventDetail.aspx/GetGiveawayTheaterInfo");
        request.Content = new StringContent($"{{giveawayIndex: '{giveawayIndex}', areaCode: '{areaCode}'}}", Encoding.UTF8, "application/json");
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(content);
        var info = obj["d"]?.ToObject<GiveawayTheaterInfo>();
        if (info == null) { throw new InvalidDataException(content); }

        foreach (var item in info.TheaterList)
        {
            item.GiveawayRemainCount = Decrypt(item.EncCount);
        }
        return info;
    }

    private string Decrypt(string data)
    {
        var bit = Convert.FromBase64String(data);
        var decryptResult = _aes.DecryptCbc(bit, _aes.IV);
        return Encoding.UTF8.GetString(decryptResult);
    }
}
