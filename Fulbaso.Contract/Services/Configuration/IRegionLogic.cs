using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IRegionLogic
    {
        void Add(Region region);

        Region Get(int regionId);

        IEnumerable<Region> Get(string name = null);
    }
}
