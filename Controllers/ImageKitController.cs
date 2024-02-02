using ImageCompressApi.Model;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace ImageCompressApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageKitController : ControllerBase
    {
        private readonly ILogger<ImageKitController> _logger;
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly string _urlEndPoint;
        public ImageKitController(
            ILogger<ImageKitController> logger)
        {
            _logger = logger;
            _privateKey = @"private_DXb3Ayz3TLeDkSRLYFndZuMstCM=";
            _publicKey = @"public_Hsf+4pvWGyhHvnTlj0gacZArLNk=";
            _urlEndPoint = "https://ik.imagekit.io/0khltgcwd";
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
        public async Task<IActionResult> UploadImage([FromForm] FileUploadModel model)
        {
            try
            {
                //var imagekit = new ImagekitClient(_publicKey, _privateKey, _urlEndPoint);
                //Stream strm = model.file.OpenReadStream();
                using var ms = new MemoryStream();
                await model.file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();


                FileCreateRequest request = new FileCreateRequest
                {
                    file = fileBytes,
                    fileName = Guid.NewGuid().ToString(),
                };
                //var result = imagekit.Upload(request);

                // Initialize the ImageKit class with your API Key and URL endpoint.

                var imagekit = new ImagekitClient(_publicKey, _privateKey, _urlEndPoint);

                // Use the Upload method to upload the file.

                var response = await imagekit.UploadAsync(request).ConfigureAwait(false);

                Console.WriteLine(response);

                await imagekit.DeleteFileAsync(response.fileId).ConfigureAwait(false);

                // The response will contain the URL of the uploaded file.
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to compress the uploaded image.");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
