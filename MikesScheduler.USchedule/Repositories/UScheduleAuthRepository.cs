using Microsoft.Extensions.Logging;
using MikesScheduler.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MikesScheduler.USchedule.Repositories;

public class UScheduleAuthRepository(ILogger logger) : IAuthRepository
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
        var loginPageResponse = await client.GetAsync("account/login");

        var requestVerificationToken = GetCookie(cookieContainer, uri, "__RequestVerificationToken");

        var loginRequest = new UScheduleLoginRequest(
            RequestVerificationToken: requestVerificationToken.Value,
            Alias: "kendallacademy",
            UserName: username,
            Password: password,
            RememberMe: false
        );

        var loginRequstJson = JsonContent.Create(loginRequest);
        
        var loginResponse = await client.PostAsync(uri, loginRequstJson);

        var aspnetSessionId = GetCookie(cookieContainer, uri, "ASP.NET_SessionId");

        var authResponse = new Auth(Cookies: [requestVerificationToken, aspnetSessionId]);

        return authResponse;
    }
}
