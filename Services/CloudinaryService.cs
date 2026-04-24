using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageCompressApi.Model;
using Microsoft.Extensions.Options;

namespace ImageCompressApi.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> options)
    {
        var cfg = options.Value;
        var account = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public ImageUploadResult UploadImage(string imagePath)
    {
        var uploadParams = new ImageUploadParams
        {
            Folder = "Product",
            File = new FileDescription(imagePath),
            Format = "webp"
        };
        return _cloudinary.Upload(uploadParams);
    }

    public void DeleteImage(string publicId)
    {
        _cloudinary.Destroy(new DeletionParams(publicId));
    }
}
