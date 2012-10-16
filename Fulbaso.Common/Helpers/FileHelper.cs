using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;

namespace Fulbaso.Common
{
    public static class FileHelper
    {
        private const string ImagesPath = "\\uploaded\\";

        public static void CopyStream(Stream input, Stream output)
        {
            input.Position = 0;
            byte[] buffer = new byte[8 * 1024];
            int len;

            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static void SaveToFile(this Stream input, string filename)
        {
            var path = FileHelper.GetPath(filename);

            if (!File.Exists(path))
            {
                using (Stream file = File.OpenWrite(path))
                {
                    CopyStream(input, file);
                }
            }
            else
            {
                throw new IOException("File already exists.");
            }
        }

        public static string GetPath(string filename)
        {
            return Path.Combine(HttpContext.Current.Server.MapPath(@"~/uploaded/"), filename);
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

        public static void DeleteFile(string filename)
        {
           var path = FileHelper.GetPath(filename);

           if (File.Exists(path))
           {
               File.Delete(path);
           }
        }

        public static string GetString(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
