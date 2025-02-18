using System.Net;
using System.Net.Http.Headers;

namespace MikesScheduler;

public record Auth(List<Cookie> Cookies);
