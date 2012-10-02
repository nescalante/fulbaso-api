using System;
using System.IO;
using System.Reflection;

namespace Fulbaso.Common
{
    public static class FileUtil
    {
        private const string ImagesPath = "\\uploaded\\";

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;

            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static void SaveToFile(this Stream input, string filename)
        {
            var path = FileUtil.GetPath(filename);

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
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

            return path.Substring(0, path.LastIndexOf('\\')) + ImagesPath + filename;
        }
    }
}
