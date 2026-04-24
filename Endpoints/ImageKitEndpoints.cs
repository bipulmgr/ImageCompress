using ImageCompressApi.Model;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;

namespace ImageCompressApi.Endpoints;

public static class ImageKitEndpoints
{
    public static void MapImageKitEndpoints(this WebApplication app)
    {
        app.MapPost("/imagekit/upload", UploadImage)
            .DisableAntiforgery()
            .WithTags("ImageKit")
            .WithSummary("Upload image to ImageKit CDN");
    }

    private static async Task<IResult> UploadImage(
        IFormFile file,
        IOptions<ImageKitSettings> settings,
        ILogger<Program> logger)
    {
        try
        {
            var cfg = settings.Value;
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var request = new FileCreateRequest
            {
                file = ms.ToArray(),
                fileName = Guid.NewGuid().ToString()
            };

            var client = new ImagekitClient(cfg.PublicKey, cfg.PrivateKey, cfg.UrlEndpoint);
            var response = await client.UploadAsync(request).ConfigureAwait(false);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upload image to ImageKit");
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}
