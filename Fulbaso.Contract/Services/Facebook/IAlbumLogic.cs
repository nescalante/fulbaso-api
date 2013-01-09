using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IAlbumLogic
    {
        Album Get(long id);

        IEnumerable<Album> Get(string page);
    }
}
