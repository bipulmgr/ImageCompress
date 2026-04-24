using ImageMagick;

namespace ImageCompressApi.Services;

public class ImageCompressionService
{
    private const int MaxDimension = 900;
    private const int JpegQuality = 75;
    private const int WebpQuality = 80;

    public Stream CompressToStream(Stream input, string fileName)
    {
        using var image = new MagickImage(input);
        ResizeIfNeeded(image);

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        image.Format = ext switch
        {
            ".png" => MagickFormat.Png,
            ".gif" => MagickFormat.Gif,
            ".webp" => MagickFormat.WebP,
            _ => MagickFormat.Jpeg
        };

        if (image.Format == MagickFormat.Jpeg)
            image.Quality = JpegQuality;

        var output = new MemoryStream();
        image.Write(output);
        output.Position = 0;
        return output;
    }

    public void CompressToFile(Stream input, string outputPath)
    {
        using var image = new MagickImage(input);
        ResizeIfNeeded(image);

        var ext = Path.GetExtension(outputPath).ToLowerInvariant();
        image.Format = ext switch
        {
            ".png" => MagickFormat.Png,
            ".gif" => MagickFormat.Gif,
            ".webp" => MagickFormat.WebP,
            _ => MagickFormat.Jpeg
        };

        if (image.Format == MagickFormat.Jpeg)
            image.Quality = JpegQuality;

        image.Write(outputPath);
    }

    public Stream ConvertToPng(Stream input)
    {
        using var image = new MagickImage(input);
        image.Format = MagickFormat.Png;
        var output = new MemoryStream();
        image.Write(output);
        output.Position = 0;
        return output;
    }

    public Stream ConvertToWebP(Stream input)
    {
        using var image = new MagickImage(input);
        image.Format = MagickFormat.WebP;
        image.Quality = WebpQuality;
        var output = new MemoryStream();
        image.Write(output);
        output.Position = 0;
        return output;
    }

    private static void ResizeIfNeeded(MagickImage image)
    {
        if (image.Width > MaxDimension || image.Height > MaxDimension)
        {
            image.Resize(new MagickGeometry(MaxDimension, MaxDimension));
        }
    }
}
