using CGVMacro;
using CGVModClient.Data;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Model;

public class CGVEventService
{
    private HttpClient client;
    private Aes aes;

    public CGVEventService()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0;) Chrome/120.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.Add("Referer", "ttps://m.cgv.co.kr/WebApp/EventNotiV4/eventMain.aspx");
        client.DefaultRequestHeaders.Host = "m.cgv.co.kr";
        client.DefaultRequestHeaders.Add("Origin", "https://m.cgv.co.kr");

        aes = Aes.Create();
        aes.IV = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNA==");
        aes.Key = Convert.FromBase64String("YjUxMWM3MWI5M2E3NDhmNDc1YzM5YzY1ZGQwZTFlOTQ=");
    }

    public async Task<GiveawayEvent[]> GetGiveawayEventsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Event/GiveawayEventList.aspx/GetGiveawayEventList");
        request.Content = new StringContent("{theaterCode: '', pageNumber: '1', pageSize: '30'}", Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
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
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(content);
        var model = obj["d"].ToObject<GiveawayEventModel>();
        if(model == null) { throw new InvalidDataException(content); }

        return model;
    }

    public async Task<GiveawayTheaterInfo> GetGiveawayTheaterInfoAsync(string giveawayIndex, int areaCode = 13)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Event/GiveawayEventDetail.aspx/GetGiveawayTheaterInfo");
        request.Content = new StringContent($"{{giveawayIndex: '{giveawayIndex}', areaCode: '{areaCode}'}}", Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(content);
        var info = obj["d"].ToObject<GiveawayTheaterInfo>();
        if (info == null) { throw new InvalidDataException(content); }

        foreach (var item in info.TheaterList)
        {
            item.GiveawayRemainCount = Decrypt(item.EncCount);
        }
        return info;
    }

    public async Task<GiveawayTheaterInfo> GetGiveawayTheaterInfoAsync(GiveawayEventModel model, int areaCode = 13)
        => await GetGiveawayTheaterInfoAsync(model.GiveawayIndex, areaCode);

    private string Decrypt(string data)
    {
        var bit = Convert.FromBase64String(data);
        var decryptResult = aes.DecryptCbc(bit, aes.IV);
        return Encoding.UTF8.GetString(decryptResult);
    }
}
