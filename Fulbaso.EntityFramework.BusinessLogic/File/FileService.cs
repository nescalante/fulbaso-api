using System;
using System.IO;
using System.Linq;
using Fulbaso.Common;
using Fulbaso.Contract;
using File = Fulbaso.Contract.File;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Fulbaso.EntityFramework.Logic
{
    public class FileService : IFileService
    {
        public void AddImage(Stream input, File file)
        {
            // validate image
            Image.FromStream(input);

            var fileName = GenerateFileName(file.CreatedBy.Id, input.Length);

            input.SaveToFile(fileName);

            var entity = new FileEntity
            {
                FileName = fileName,
                Description = file.Description,
                ContentLength = file.ContentLength,
                ContentType = file.ContentType,
                UserId = file.CreatedBy.Id,
                InsertDate = DateTime.Now,
            };

            EntityUtil.Context.Files.AddObject(entity);
            EntityUtil.Context.SaveChanges();

            file.Id = entity.Id;
        }

        public File AddImage(string source, string description, long userId)
        {
            File file;

            using (var input = FileHelper.GetStreamFromUrl(source))
            {
                file = new File
                {
                    Description = description,
                    ContentLength = Convert.ToInt32(input.Length),
                    ContentType = "image/jpeg",
                    CreatedBy = new User { Id = userId },
                };

                AddImage(input, file);
            }

            return file;
        }

        private string GenerateFileName(long userId, long length)
        {
            var now = DateTime.Now;
            var last = EntityUtil.Context.Files.Select(f => f.Id).OrderByDescending(f => f).FirstOrDefault() + 1;
            var name = "o.jpg";

            return string.Join("_", new object [] {
                userId,
                now.Year,
                now.Month,
                now.Day,
                last,
                length,
                name,
            });
        }
    }
}
