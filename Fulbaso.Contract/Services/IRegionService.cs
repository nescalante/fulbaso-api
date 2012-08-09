using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IRegionService
    {
        Region Get(int regionId);

        IEnumerable<Region> Get(string name = null);
    }
}
