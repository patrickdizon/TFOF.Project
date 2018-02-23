using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;


namespace TFOF.Areas.Core.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap ProcessAvatar(this Bitmap image, string filePath)
        {
            return image.ReSize(101, 80, filePath).Crop(100, 100, filePath);
            
        }
        public static Bitmap Crop(this Bitmap b, int width, int height, string filePath)
        {
            Bitmap nb = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(nb);
            int x = 0;
            int y = 0;
            if(b.Width > width)
            {
                x = (b.Width - width) / 2;
            } else
            {
                width = b.Width;

            }
            if(b.Height > height)
            {
                y = (b.Height - height) / 2;

            } else
            {
                height = b.Height;
            }
            g.DrawImage(b, -x-1, -y-1);
            nb.Save(filePath);
            return nb;
        }

        public static Bitmap ReSize(this Bitmap image, int length, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;
            
            int newWidth = (image.Width < image.Height) ? length : (int)(image.Width * ((float)length / (float)image.Height));
            int newHeight = (image.Width < image.Height) ? (int)(image.Height * ((float)length / (float)image.Width)) : length;
            
            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            Encoder encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
            return newImage;
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

    }
}