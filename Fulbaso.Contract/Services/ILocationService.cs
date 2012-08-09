using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ILocationService
    {
        Location Get(int locationId);

        IEnumerable<Location> Get(string name = null);

        IEnumerable<Location> GetByRegion(int regionId);

        IEnumerable<string> GetForAutocomplete(string prefixText, int count);
    }
}
