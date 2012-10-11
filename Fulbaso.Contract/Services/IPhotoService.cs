using System.Collections.Generic;
using Fulbaso.Contract;

namespace Fulbaso.Contract
{
    public interface IPhotoService
    {
        Photo Get(long id);

        IEnumerable<Photo> GetFromAlbum(long id);
    }
}
