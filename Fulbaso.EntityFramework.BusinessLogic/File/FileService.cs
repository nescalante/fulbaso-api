using System;
using System.IO;
using System.Linq;
using Fulbaso.Common;
using Fulbaso.Contract;
using File = Fulbaso.Contract.File;

namespace Fulbaso.EntityFramework.Logic
{
    public class FileService : IFileService
    {
        public void AddImage(Stream input, File file)
        {
            var fileName = GenerateFileName(file.FileName, file.CreatedBy.Id);

            input.SaveToFile(FileUtil.GetDirectory() + fileName);

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

        private string GenerateFileName(string fileName, long userId)
        {
            var now = DateTime.Now;
            var last = EntityUtil.Context.Files.Select(f => f.Id).OrderByDescending(f => f).FirstOrDefault();
            var name = new string(fileName.Split('.').First().Take(40).ToArray()) + "." + fileName.Split('.').Last();

            return string.Join("_", new object [] {
                userId,
                now.Year,
                now.Month,
                now.Day,
                last,
                name,
            });
        }
    }
}
