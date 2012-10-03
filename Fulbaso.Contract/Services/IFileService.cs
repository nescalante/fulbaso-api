using System.IO;

namespace Fulbaso.Contract
{
    public interface IFileService
    {
        void AddImage(Stream input, File file);

        File AddImage(string source, string description, long userId);
    }
}
