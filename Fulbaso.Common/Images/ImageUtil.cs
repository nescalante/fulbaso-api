using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using File = Fulbaso.Contract.File;
using System.Web.Mvc;
using System.Web;

namespace Fulbaso.Common
{
    public static class ImageUtil
    {
        public static string ToUrl(this File file, int width = 0)
        {


            var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            return helper.Content("~/uploaded/" + file.FileName);
        }

        public static Image GetImageFromUrl(string url)
        {
            using (MemoryStream ms = ImageUtil.GetStreamFromUrl(url))
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

        public static MemoryStream GetStreamFromUrl(string url)
        {
            var http = "http://";
            var https = "https://";
            var imageData = new byte[1];
            Uri uri;

            url = url.Replace(http, string.Empty).Replace(https, string.Empty);
            url = http + url;

            // changed to UriKind.Absolute to catch empty string
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                using (WebClient client = new WebClient())
                {
                    imageData = client.DownloadData(uri);
                    return new MemoryStream(imageData);
                }
            }
            else
            {
                throw new ArgumentException("Invalid URL.");
            }
        }
    }
}
