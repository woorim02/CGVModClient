using Microsoft.Maui.Controls.Handlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CGVModClient.Model;

public class CgvAuthService
{
    private readonly HttpClient _authClient;
    private readonly SocketsHttpHandler _authHandler;
    private readonly Aes _aes;
    private readonly SHA256 _sha256;
    private readonly MD5 _md5;

    public CgvAuthService(HttpClient authClient, SocketsHttpHandler handler, Aes aes, SHA256 sha256, MD5 md5)
    {
        _authClient = authClient;
        _authHandler = handler;
        _aes = aes;
        _sha256 = sha256;
        _md5 = md5;
    }

    public async Task<bool> LoginAsync(string id, string password)
    {
        var getCookieResponse = await _authClient.GetAsync("https://m.cgv.co.kr/Webapp/Member/Login.aspx");
        getCookieResponse.EnsureSuccessStatusCode();
        var checkIpResponse = await _authClient.GetAsync("https://m.cgv.co.kr/WebAPP/Member/Login.aspx/CheckIP");
        checkIpResponse.EnsureSuccessStatusCode();
        var captchaResponse = await _authClient.GetAsync("https://m.cgv.co.kr/WebAPP/Member/Login.aspx/InputCheckCaptcha");
        captchaResponse.EnsureSuccessStatusCode();

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

        var response = await _authClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        if (content.Contains("ID 또는 비밀번호가 일치하지 않습니다"))
            return false;

        var document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(content);
        var uri = document.DocumentNode.FirstChild.Attributes["src"].Value;
        var setCookieResponse = _authClient.GetAsync(uri);
        return true;
    }

    public IEnumerable<Cookie> GetAuthCookies()
    {
        return SelectAuthCookies(_authHandler.CookieContainer.GetAllCookies()); 
    }

    public void SetAuthCookies(IEnumerable<Cookie> cookies)
    {
        ArgumentNullException.ThrowIfNull(cookies, nameof(cookies));
        foreach (var cookie in SelectAuthCookies(cookies))
        {
            _authHandler.CookieContainer.Add(cookie);
        }
    }

    private static IEnumerable<Cookie> SelectAuthCookies(IEnumerable<Cookie> cookies)
    {
        ArgumentNullException.ThrowIfNull(cookies, nameof(cookies));
        var selectedCookies = cookies.Where(x =>
        {
            switch (x.Name)
            {
                case "ASP.NET_SessionId":
                    return true;
                case "WEBAUTH":
                    return true;
                case "AUTOLOGIN":
                    return true;
                case ".ASPXAUTH":
                    return true;
                case "URL_PREV_COMMON":
                    return true;
                case "REURL":
                    return true;
                default: return false;
            }
        });
        if (selectedCookies.Count() < 4)
            return Enumerable.Empty<Cookie>();
        return selectedCookies;
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
