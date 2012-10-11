using System.Collections.Generic;
using Fulbaso.Contract;

namespace Fulbaso.Contract
{
    public interface IAlbumService
    {
        Album Get(long id);

        IEnumerable<Album> Get(string page);
    }
}
