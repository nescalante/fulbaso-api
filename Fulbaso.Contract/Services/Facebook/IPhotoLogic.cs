using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IPhotoLogic
    {
        Photo Get(long id);

        IEnumerable<Photo> GetFromAlbum(long id);
    }
}
