using Microsoft.Maui.Controls.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Model;

public class CgvAuthService
{
    private readonly HttpClient _client;
    private readonly SocketsHttpHandler _handler;
    private Aes _aes;
    private SHA256 _sha256;
    private MD5 _md5;

    public CgvAuthService(HttpClient authClient, SocketsHttpHandler handler, Aes aes, SHA256 sha256, MD5 md5)
    {
        _client = authClient;
        _handler = handler;
        _aes = aes;
        _sha256 = sha256;
        _md5 = md5;
    }

    public async Task<bool> LoginAsync(string id, string password)
    {
        var getCookieResponse = await _client.GetAsync("https://m.cgv.co.kr/Webapp/Member/Login.aspx");
        getCookieResponse.EnsureSuccessStatusCode();
        var checkIpResponse = await _client.GetAsync("https://m.cgv.co.kr/WebAPP/Member/Login.aspx/CheckIP");
        checkIpResponse.EnsureSuccessStatusCode();
        var captchaResponse = await _client.GetAsync("https://m.cgv.co.kr/WebAPP/Member/Login.aspx/InputCheckCaptcha");
        checkIpResponse?.EnsureSuccessStatusCode();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://m.cgv.co.kr/Webapp/Member/Login.aspx");
        var form = new Dictionary<string, string>()
        {
            { "hfUserId", Uri.EscapeDataString(Encrypt(id.Trim())) },
            { "hfPasswordInter", Uri.EscapeDataString(ComputeSha256Hash(password)) },
            { "hfPasswordLocal", Uri.EscapeDataString(ComputeSha256Hash(ComputeMD5Hash(password))) },
            { "hfReUrl", Uri.EscapeDataString(Encrypt("https %3a%2f%2fm.cgv.co.kr%2f")) },
            { "hfAgree", Uri.EscapeDataString(Encrypt("1")) },
            { "nonmemberStateCd", Uri.EscapeDataString(Encrypt("0")) }
        };
        request.Content = new FormUrlEncodedContent(form);

        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (content.Contains("ID 또는 비밀번호가 일치하지 않습니다"))
            return false;

        var document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(content);
        var uri = document.DocumentNode.FirstChild.Attributes["src"].Value;
        var setCookieResponse = _client.GetAsync(uri);
        return true;
    }

    public async Task SaveAuthStateAsync()
    {
        var json = JsonConvert.SerializeObject(_handler.CookieContainer.GetAllCookies());
        await File.WriteAllTextAsync(Environment.CurrentDirectory + "\\cookie.json", json);
    }

    public async Task LoadAuthStateAsync()
    {
        var json = await File.ReadAllTextAsync(Environment.CurrentDirectory + "\\cookie.json");
        var cookies = JsonConvert.DeserializeObject<CookieCollection>(json);
        foreach (Cookie item in cookies)
        {
            _handler.CookieContainer.Add(item);
        }
    }


    private string Encrypt(string data)
    {
        var bit = Encoding.UTF8.GetBytes(data);
        var encryptResult = _aes.EncryptCbc(bit, _aes.IV);
        return Convert.ToBase64String(encryptResult);
    }

    private string Decrypt(string data)
    {
        var bit = Convert.FromBase64String(data);
        var decryptResult = _aes.DecryptCbc(bit, _aes.IV);
        return Encoding.UTF8.GetString(decryptResult);
    }

    private string ComputeSha256Hash(string data)
    {
        return Convert.ToHexString(_sha256.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }

    private string ComputeMD5Hash(string data)
    {
        return Convert.ToHexString(_md5.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }
}
