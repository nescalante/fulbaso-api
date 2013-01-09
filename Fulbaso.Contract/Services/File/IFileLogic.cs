using System.IO;

namespace Fulbaso.Contract
{
    public interface IFileLogic
    {
        void AddImage(Stream input, File file);

        File AddImage(string source, string description, long userId);

        void Delete(int fileId);
    }
}
