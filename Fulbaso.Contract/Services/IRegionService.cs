using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IRegionService
    {
        void Add(Region region);

        Region Get(int regionId);

        IEnumerable<Region> Get(string name = null);
    }
}
