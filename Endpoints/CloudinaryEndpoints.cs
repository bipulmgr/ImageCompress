using ImageCompressApi.Services;

namespace ImageCompressApi.Endpoints;

public static class CloudinaryEndpoints
{
    public static void MapCloudinaryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/cloudinary").WithTags("Cloudinary");

        group.MapPost("/upload", UploadImage)
            .DisableAntiforgery()
            .WithSummary("Upload image to Cloudinary");

        group.MapDelete("/{publicId}", DeleteImage)
            .WithSummary("Delete image from Cloudinary");
    }

    private static async Task<IResult> UploadImage(IFormFile file, CloudinaryService cloudinaryService)
    {
        if (file.Length == 0)
            return Results.BadRequest(new { message = "No image file provided." });

        var tempPath = Path.GetTempFileName();
        try
        {
            await using (var stream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var result = cloudinaryService.UploadImage(tempPath);
            return Results.Ok(new { url = result.SecureUrl.ToString() });
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    private static IResult DeleteImage(string publicId, CloudinaryService cloudinaryService)
    {
        cloudinaryService.DeleteImage(publicId);
        return Results.Ok();
    }
}
