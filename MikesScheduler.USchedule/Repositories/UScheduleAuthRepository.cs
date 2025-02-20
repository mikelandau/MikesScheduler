using Microsoft.Extensions.Logging;
using MikesScheduler.Repositories;
using System.Net;
using System.Net.Http.Json;

namespace MikesScheduler.USchedule.Repositories;

public class UScheduleAuthRepository(ILogger<UScheduleAuthRepository> logger) : IAuthRepository
{

    private Cookie GetCookie(CookieContainer cookieContainer, Uri uri, string name)
    {
        var cookie = cookieContainer.GetCookies(uri).Cast<Cookie>().FirstOrDefault(x => x.Name == name)
            ?? throw new Exception($"Could not get {name} cookie from login page response");

        return cookie;

    }

    public async Task<Auth> Login(string username, string password)
    {   
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer
        };
        using var client = new HttpClient(handler);

        var uri = new Uri("https://clients.uschedule.com/kendallacademy/account/login");

        // load the login page to save the __RequestVerificationToken cookie
        var loginPageResponse = await client.GetAsync(uri);

        var requestVerificationToken = GetCookie(cookieContainer, uri, "__RequestVerificationToken");

        logger.LogWarning("Got __RequestVerificationToken {TokenValue}", requestVerificationToken.Value);

        var uScheduleLoginRequest = new List<KeyValuePair<string, string>>
        {
            new("__RequestVerificationToken", requestVerificationToken.Value),
            new("Alias", "kendallacademy"),
            new("UserName", username),
            new("Password", password),
            new("RememberMe", "false")
        };

        var loginResponse = await client.PostAsync(uri, new FormUrlEncodedContent(uScheduleLoginRequest));

        var aspnetSessionId = GetCookie(cookieContainer, uri, "ASP.NET_SessionId");

        var authResponse = new Auth(Cookies: [requestVerificationToken, aspnetSessionId]);

        return authResponse;
    }
}
