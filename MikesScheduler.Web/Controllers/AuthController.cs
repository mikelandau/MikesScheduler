using Microsoft.AspNetCore.Mvc;
using MikesScheduler.Repositories;
using System.Net.Http.Headers;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MikesScheduler.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthRepository authRepository) : ControllerBase
{

    // GET: api/<AuthController>
    [HttpGet]
    public async Task<HttpResponseMessage> Get(string username, string password)
    {
        var auth = await authRepository.Login(username, password);

        var resp = new HttpResponseMessage();
        resp.StatusCode = System.Net.HttpStatusCode.NoContent;
        resp.Headers.AddCookies([.. auth.Cookies.Select(cookie => new CookieHeaderValue(cookie.Name, cookie.Value))]);

        return resp;
    }
}
