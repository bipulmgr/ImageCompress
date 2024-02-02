using ImageCompressApi.Model;
using ImageCompressApi.Util;
using Libwebp.Net;
using Libwebp.Net.utility;
using Libwebp.Standard;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Text;

namespace ImageCompressApi.Controllers
{

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

        //[HttpGet]
        //public IActionResult Test([FromQuery] string hexa)
        //{
        //    try
        //    {
        //        var strByte = StringToByteArray(hexa);
        //        for (var i = 0; i < strByte.Length; i++)
        //        {
        //            strByte[i] /= 2;
        //        }
        //        var str = ByteArrayToString(strByte);
        //        return Ok(str);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[HttpGet("Hello")]
        //public IActionResult Hello([FromQuery] string hexa)
        //{
        //    try
        //    {
        //        return Ok(ConvertToString(hexa));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost("ConvertToPng")]
        public IActionResult Convert([FromForm] FileUploadModel model)
        {
            try
            {
                Bitmap b = (Bitmap)Bitmap.FromStream(model.file.OpenReadStream());
                using MemoryStream ms = new MemoryStream();

                b.Save(ms, ImageFormat.Png);
                return File(ms.ToArray(), "image/png", Path.GetFileName("oNGImAGE"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Compress image api
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
                string fileUrl = Path.Combine(uploadFolder, $"{model.file.FileName}");
                Stream stream = model.file.OpenReadStream();
                CompressImage.Compress(stream, fileUrl);
                return Ok(new { message = "Compressed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to compress the uploaded image.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CompressWithDownload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [RequestSizeLimit(104857600)]
        public IActionResult CompressWithDownload([FromForm] FileUploadModel model)
        {
            try
            {
                //using (Stream stream = model.file.OpenReadStream())
                //{
                //    using var compressedImageStream = CompressImage.CompressImageStream(stream);

                //    // Make sure to set the position of the MemoryStream back to the beginning
                //    compressedImageStream.Position = 0;

                //    var extension = Path.GetExtension(model.file.FileName).ToLower();
                //    var contentType = GetContentType(extension);

                //    return File(compressedImageStream, contentType, model.file.FileName);
                //}
                using (Stream stream = model.file.OpenReadStream())
                {
                    using var compressedImageStream = CompressImage.CompressImageStream(stream);

                    // Create a new MemoryStream and copy the compressed image data into it
                    var memoryStream = new MemoryStream();
                    compressedImageStream.CopyTo(memoryStream);

                    // Make sure to set the position of the new MemoryStream back to the beginning
                    memoryStream.Position = 0;

                    var extension = Path.GetExtension(model.file.FileName).ToLower();
                    var contentType = GetContentType(extension);

                    return File(memoryStream, contentType, model.file.FileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error compressing and downloading image: " + ex.Message);
            }
        }

        //private string ConvertToString(string hexa)
        //{
        //    var str = Convert.FromHexString(hexa);
        //    var outputStr = string.Empty;
        //    for (var i = 0; i < str.Length; i++)
        //    {
        //        str[i] /= 2;
        //    }
        //    return Convert.ToHexString(str);
        //}

        //private static byte[] StringToByteArray(string hex)
        //{
        //    int NumberChars = hex.Length;
        //    byte[] bytes = new byte[NumberChars / 2];
        //    for (int i = 0; i < NumberChars; i += 2)
        //        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        //    return bytes;
        //}
        private static string ByteArrayToString(byte[] byteArray)
        {
            StringBuilder hexa = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
                hexa.AppendFormat("{0:x2}", b);
            return hexa.ToString();
        }

        [HttpPost("ConvertToWebp")]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> UploadAsync([FromForm] FileUploadModel model)
        {
            if (model.file == null) throw new FileNotFoundException();
            //you can handle file checks ie. extensions, file size etc..
            //creating output file name
            // your name can be a unique Guid or any name of your choice with .webp extension..eg output.webp
            //in my case i am removing the extension from the uploaded file and appending
            // the .webp extension
            var oFileName = $"{Path.GetFileNameWithoutExtension(model.file.FileName)}.webp";
            // create your webp configuration
            var config = new WebpConfigurationBuilder().Preset(Preset.PHOTO).Output(oFileName).Build();
            //create an encoder and pass in your configuration
            var encoder = new WebpEncoder(config);
            //copy file to memory stream
            var ms = new MemoryStream();
            model.file.CopyTo(ms);
            //call the encoder and pass in the MemoryStream and input FileName
            //the encoder after encoding will return a FileStream output
            //Optional cast to Stream to return file for download
            Stream fs = await encoder.EncodeAsync(ms, model.file.FileName);
            /*Do whatever you want with the file....download, copy to disk or
              save to cloud*/
            return File(fs, "application/octet-stream", oFileName);
        }

        [HttpPost("BulkConvertToWebp")]
        public async Task<IActionResult> BulkConvertToWebp(List<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                    return BadRequest("No images were uploaded.");

                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot");

                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                foreach (var image in images)
                {
                    var oFileName = $"{Path.GetFileNameWithoutExtension(image.FileName)}.webp";
                    var filePath = Path.Combine(rootPath, oFileName);
                    var config = new WebpConfigurationBuilder().Preset(Preset.PHOTO).Output(oFileName).Build();
                    var encoder = new WebpEncoder(config);
                    var ms = new MemoryStream();
                    image.CopyTo(ms);
                    Stream fs = await encoder.EncodeAsync(ms, image.FileName);
                    using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    await fs.CopyToAsync(fileStream);
                }
                return Ok(new { message = "Converted successfully" });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet("download")]
        public IActionResult DownloadFiles()
        {
            var filesToDownload = new List<string>
        {
            "path/to/file1.txt",
            "path/to/file2.txt",
            // Add other file paths here
        };

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filesToDownload)
                    {
                        var fileEntry = archive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Optimal);

                        using (var fileStream = new FileStream(filePath, FileMode.Open))
                        using (var entryStream = fileEntry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;
                var archiveName = "files.zip"; // The name of the zip archive

                return File(memoryStream, "application/octet-stream", archiveName);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages(List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                return BadRequest("No images were uploaded.");

            var imageUrls = new List<string>();

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine("path/to/upload/directory", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var imageUrl = $"https://example.com/{fileName}"; // Replace with the appropriate URL for accessing the uploaded image
                    imageUrls.Add(imageUrl);
                }
            }

            return Ok(imageUrls);
        }
        private string GetContentType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                default:
                    return "application/octet-stream"; // Default to binary if not recognized
            }
        }
    }
}