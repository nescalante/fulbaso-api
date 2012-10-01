using System.IO;

namespace Fulbaso.Contract
{
    public interface IFileService
    {
        void AddImage(Stream input, File file);
    }
}
