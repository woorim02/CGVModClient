using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Model;

public class CgvReservationService
{
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public CgvReservationService(HttpClient defaultClient, HttpClient authClient)
    {
        _client = defaultClient;
        _authClient = authClient;
    }

    public async Task<TheaterScheduleListRoot?> GetScheduleListAsync(string theaterCode, DateTime date, string screenTypeCode = "02")
        => await GetScheduleListAsync(theaterCode, "", date, screenTypeCode);

    public async Task<TheaterScheduleListRoot?> GetScheduleListAsync(string theaterCode, string movieGroupCd, DateTime date, string screenTypeCode = "02")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://m.cgv.co.kr/WebApp/Reservation/Schedule.aspx");
        await _client.SendAsync(request);

        request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/WebAPP/Reservation/Common/ajaxTheaterScheduleList.aspx/GetTheaterScheduleList");
        request.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
        request.Headers.Add("Accept-Language", "ko-KR,ko;q=0.8,en-US;q=0.5,en;q=0.3");
        request.Headers.Add("Origin", "https://m.cgv.co.kr");
        request.Headers.Add("Cookie", "URL_PREV_COMMON=");
        var body = new
        {
            strRequestType = "THEATER",
            strUserID = "",
            strMovieGroupCd = movieGroupCd,
            strMovieTypeCd = "",
            strPlayYMD = $"{date:yyyyMMdd}",
            strTheaterCd = theaterCode,
            strScreenTypeCd = screenTypeCode,
            strRankType = "MOVIE",
        };
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        content = JObject.Parse(content)["d"]?.ToString();
        var root = JsonConvert.DeserializeObject<TheaterScheduleListRoot>(content);
        return root;
    }
}
