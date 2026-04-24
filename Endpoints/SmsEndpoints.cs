using ImageCompressApi.Model;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace ImageCompressApi.Endpoints;

public static class SmsEndpoints
{
    public static void MapSmsEndpoints(this WebApplication app)
    {
        app.MapPost("/sms/send-otp", SendOtp)
            .WithTags("SMS")
            .WithSummary("Send OTP via SparrowSMS");
    }

    private static async Task<IResult> SendOtp(
        string to,
        IOptions<SmsSettings> settings,
        ILogger<Program> logger)
    {
        try
        {
            var cfg = settings.Value;
            var otp = Random.Shared.Next(0, 1_000_000).ToString("D6");
            var text = $"Your OTP code is {otp}";

            var client = new RestClient(cfg.Url);
            var request = new RestRequest(cfg.Url, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("token", cfg.Token);
            request.AddParameter("from", cfg.From);
            request.AddParameter("to", to);
            request.AddParameter("text", text);

            var response = await client.PostAsync(request).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonSerializer.Deserialize<SmsResponse>(response.Content ?? "{}",
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return Results.Ok(new { message = result?.Response });
            }

            return Results.BadRequest(new { message = response.ErrorMessage });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send OTP");
            return Results.Problem(ex.Message);
        }
    }

    private record SmsResponse(string Response, int Code, int Count);
}
