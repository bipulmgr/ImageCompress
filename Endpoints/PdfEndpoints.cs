using ImageCompressApi.Model;
using ImageCompressApi.Services;

namespace ImageCompressApi.Endpoints;

public static class PdfEndpoints
{
    public static void MapPdfEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/pdf").WithTags("PDF");

        group.MapPost("/upload", UploadAndExtract)
            .DisableAntiforgery()
            .WithSummary("Upload PDF and extract questions");
    }

    private static async Task<IResult> UploadAndExtract(IFormFile file, PdfService pdfService)
    {
        if (file.Length == 0)
            return Results.BadRequest(new { message = "File not provided or empty." });

        var tempPath = $"{Path.GetTempFileName()}.pdf";
        try
        {
            await using (var fileStream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            var questions = pdfService.ExtractQuestions(tempPath);
            return Results.Ok(questions);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
