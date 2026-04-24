using ImageCompressApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageCompressApi.Endpoints;

public static class ImageCompressEndpoints
{
    public static void MapImageCompressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/imagecompress").WithTags("Image Compress");

        group.MapPost("/", CompressAndSave)
            .DisableAntiforgery()
            .WithSummary("Compress image and save to server");

        group.MapPost("/compress-download", CompressAndDownload)
            .DisableAntiforgery()
            .WithSummary("Compress image and return as download");

        group.MapPost("/to-png", ConvertToPng)
            .DisableAntiforgery()
            .WithSummary("Convert image to PNG");

        group.MapPost("/to-webp", ConvertToWebP)
            .DisableAntiforgery()
            .WithSummary("Convert image to WebP");

        group.MapPost("/bulk-to-webp", BulkConvertToWebP)
            .DisableAntiforgery()
            .WithSummary("Convert multiple images to WebP and save to server");
    }

    private static IResult CompressAndSave(
        IFormFile file,
        ImageCompressionService compressionService,
        ILogger<Program> logger)
    {
        try
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadFolder);

            var outputPath = Path.Combine(uploadFolder, file.FileName);
            using var stream = file.OpenReadStream();
            compressionService.CompressToFile(stream, outputPath);

            return Results.Ok(new { message = "Compressed successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to compress image");
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static IResult CompressAndDownload(IFormFile file, ImageCompressionService compressionService)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var compressed = compressionService.CompressToStream(stream, file.FileName);

            var contentType = GetContentType(Path.GetExtension(file.FileName));
            return Results.File(compressed, contentType, file.FileName);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static IResult ConvertToPng(IFormFile file, ImageCompressionService compressionService)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var converted = compressionService.ConvertToPng(stream);
            var outputName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.png";
            return Results.File(converted, "image/png", outputName);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static IResult ConvertToWebP(IFormFile file, ImageCompressionService compressionService)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var converted = compressionService.ConvertToWebP(stream);
            var outputName = $"{Path.GetFileNameWithoutExtension(file.FileName)}.webp";
            return Results.File(converted, "image/webp", outputName);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> BulkConvertToWebP(
        IFormFileCollection images,
        ImageCompressionService compressionService)
    {
        if (images.Count == 0)
            return Results.BadRequest(new { message = "No images uploaded." });

        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        Directory.CreateDirectory(rootPath);

        foreach (var image in images)
        {
            await using var stream = image.OpenReadStream();
            var converted = compressionService.ConvertToWebP(stream);
            var outputPath = Path.Combine(rootPath, $"{Path.GetFileNameWithoutExtension(image.FileName)}.webp");
            await using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            await converted.CopyToAsync(fileStream);
        }

        return Results.Ok(new { message = "Converted successfully" });
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
