using ImageCompressApi.Model;
using ImageCompressApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImageCompressApi.Controllers;

[ApiController]
[Route("[controller]")]

public class ImageController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public ImageController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost]
    public IActionResult UploadImage([FromForm] FileUploadModel model)
    {
        if (model.file != null && model.file.Length > 0)
        {
            // Save the file to a temporary location
            var tempImagePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempImagePath, FileMode.Create))
            {
                model.file.CopyTo(stream);
            }

            // Upload the image to Cloudinary
            var result = _cloudinaryService.UploadImage(tempImagePath);

            // Cleanup the temporary file
            System.IO.File.Delete(tempImagePath);

            // Handle the result, e.g., store the Cloudinary URL or perform additional actions
            var imageUrl = result.SecureUri.ToString();

            // Return the image URL or any other response as needed
            return Ok(imageUrl);
        }

        return BadRequest("No image file provided");
    }

    [HttpDelete]
    public IActionResult DeleteImage(string publicId)
    {
        // Call the Cloudinary service to delete the image
        _cloudinaryService.DeleteImage(publicId);

        // Return a success response or perform additional actions
        return Ok();
    }
}
