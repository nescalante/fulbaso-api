using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ImageFile = Fulbaso.Contract.File;

namespace Fulbaso.Common
{
    public static class ImageHelper
    {
        private static string GetUrl(string fileName)
        {
            var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            return helper.Content("~/uploaded/" + fileName);
        }

        private static int GetTargetHeight(int originalWidth, int targetWidth, int originalHeight)
        {
            return Convert.ToInt32(decimal.Round((decimal)targetWidth / originalWidth * originalHeight, 0));
        }

        private static string GetFileName(ImageFile file, int width)
        {
            return string.Join("_", new object[] {
                file.CreatedBy.Id,
                file.InsertDate.Year,
                file.InsertDate.Month,
                file.InsertDate.Day,
                file.Id,
                width,
                "r.jpg",
            });
        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public static string ToUrl(this ImageFile file, int width = 0)
        {
            if (width > 0)
            {
                var fileName = GetFileName(file, width);

                if (File.Exists(FileHelper.GetPath(fileName)))
                {
                    return ImageHelper.GetUrl(fileName);
                }

                try
                {
                    var source = FileHelper.GetPath(file.FileName);
                    var img = Image.FromFile(source);

                    if (img.Width > width)
                    {
                        var bitmap = new Bitmap(width, GetTargetHeight(img.Width, width, img.Height));
                        var g = Graphics.FromImage(bitmap);

                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(img, 0, 0, width, GetTargetHeight(img.Width, width, img.Height));
                        g.Dispose();

                        FileHelper.SaveToFile(bitmap.ToStream(ImageFormat.Jpeg), fileName);

                        return ImageHelper.GetUrl(fileName);
                    }
                }
                catch { }
            }
                
            return ImageHelper.GetUrl(file.FileName);
        }

        public static Image GetImageFromUrl(string url)
        {
            using (MemoryStream ms = FileHelper.GetStreamFromUrl(url))
            {
                return Image.FromStream(ms);
            }
        }

        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
