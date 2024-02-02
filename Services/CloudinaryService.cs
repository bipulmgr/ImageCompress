using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageCompressApi.Model;
using Microsoft.Extensions.Options;

namespace ImageCompressApi.Services;


public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig)
    {
        var account = new Account(
            cloudinaryConfig.Value.CloudName,
            cloudinaryConfig.Value.ApiKey,
            cloudinaryConfig.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public ImageUploadResult UploadImage(string imagePath)
    {
        var uploadParams = new ImageUploadParams
        {
            Folder="Product",
            File = new FileDescription(imagePath),
            Format = "webp"
        };

        return _cloudinary.Upload(uploadParams);
    }

    public void DeleteImage(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        _cloudinary.Destroy(deleteParams);
    }
}
