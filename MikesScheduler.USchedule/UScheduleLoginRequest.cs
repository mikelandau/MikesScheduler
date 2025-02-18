using System.Text.Json.Serialization;

namespace MikesScheduler.USchedule;

record UScheduleLoginRequest(
    [property: JsonPropertyName("__RequestVerificationToken")] string RequestVerificationToken,
    string Alias,
    string UserName,
    string Password,
    bool RememberMe);
