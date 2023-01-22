using ImageCompressApi.Util;
using Microsoft.AspNetCore.Mvc;

namespace ImageCompressApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageCompressController : ControllerBase
{
    private readonly ILogger<ImageCompressController> _logger;
    public ImageCompressController(
        ILogger<ImageCompressController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Comprees image api
    /// the  file is upload in wwwroot folder
    /// </summary>
    /// <param name="model">the instance of <see cref="FileUploadModel" /> </param>
    /// <returns></returns>

    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    [RequestSizeLimit(104857600)]
    public IActionResult UploadImage([FromForm] FileUploadModel model)
    {
        try
        {
            // this is the directory where file is upload
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            // this is the place where file is uploaded
            string fileUrl = Path.Combine(uploadFolder, $"{Guid.NewGuid().ToString()}_{model.file.FileName}");
            Stream strm = model.file.OpenReadStream();
            CompressImage.Compressimage(strm, fileUrl);
            return Ok(new { message = "Compressed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public class FileUploadModel
    {
        public IFormFile file { get; set; }
    }
}
