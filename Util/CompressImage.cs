using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageCompressApi.Util
{
    public class CompressImage
    {
        public static void Compress(Stream srcImgStream, string targetPath)
        {
            try
            {
                // Convert stream to image
                using var image = Image.FromStream(srcImgStream);

                float maxHeight = 900.0f;
                float maxWidth = 900.0f;
                int newWidth;
                int newHeight;

                var originalBMP = new Bitmap(srcImgStream);
                int originalWidth = originalBMP.Width;
                int originalHeight = originalBMP.Height;

                if (originalWidth > maxWidth || originalHeight > maxHeight)
                {
                    // To preserve the aspect ratio  
                    float ratioX = (float)maxWidth / (float)originalWidth;
                    float ratioY = (float)maxHeight / (float)originalHeight;
                    float ratio = Math.Min(ratioX, ratioY);
                    newWidth = (int)(originalWidth * ratio);
                    newHeight = (int)(originalHeight * ratio);
                }
                else
                {
                    newWidth = (int)originalWidth;
                    newHeight = (int)originalHeight;
                }

                var bitmap = new Bitmap(originalBMP, newWidth, newHeight);
                var imgGraph = Graphics.FromImage(bitmap);

                imgGraph.SmoothingMode = SmoothingMode.Default;
                imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

                var extension = Path.GetExtension(targetPath).ToLower();
                // for file extension having png and gif
                if (extension == ".png" || extension == ".gif")
                {
                    // Save image to targetPath
                    bitmap.Save(targetPath, image.RawFormat);
                }

                // for file extension having .jpg or .jpeg
                else if (extension == ".jpg" || extension == ".jpeg")
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    Encoder myEncoder = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1);
                    var parameter = new EncoderParameter(myEncoder, 40L);
                    encoderParameters.Param[0] = parameter;

                    // Save image to targetPath
                    bitmap.Save(targetPath, jpgEncoder, encoderParameters);
                }
                bitmap.Dispose();
                imgGraph.Dispose();
                originalBMP.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Stream CompressImageStream(Stream srcImgStream)
        {
            try
            {
                // Convert stream to image
                using var image = Image.FromStream(srcImgStream);

                float maxHeight = 900.0f;
                float maxWidth = 900.0f;
                int newWidth;
                int newHeight;

                var originalBMP = new Bitmap(srcImgStream);
                int originalWidth = originalBMP.Width;
                int originalHeight = originalBMP.Height;

                if (originalWidth > maxWidth || originalHeight > maxHeight)
                {
                    // To preserve the aspect ratio  
                    float ratioX = (float)maxWidth / (float)originalWidth;
                    float ratioY = (float)maxHeight / (float)originalHeight;
                    float ratio = Math.Min(ratioX, ratioY);
                    newWidth = (int)(originalWidth * ratio);
                    newHeight = (int)(originalHeight * ratio);
                }
                else
                {
                    newWidth = (int)originalWidth;
                    newHeight = (int)originalHeight;
                }

                var bitmap = new Bitmap(originalBMP, newWidth, newHeight);
                var imgGraph = Graphics.FromImage(bitmap);

                imgGraph.SmoothingMode = SmoothingMode.Default;
                imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

                // Create a MemoryStream to store the compressed image
                var compressedImageStream = new MemoryStream();

                // Save the compressed image to the MemoryStream
                bitmap.Save(compressedImageStream, ImageFormat.Jpeg); // Update the format as needed

                // Set the position of the MemoryStream back to the beginning
                compressedImageStream.Position = 0;

                // Dispose unnecessary resources
                bitmap.Dispose();
                imgGraph.Dispose();
                originalBMP.Dispose();

                // Return the MemoryStream containing the compressed image
                return compressedImageStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}