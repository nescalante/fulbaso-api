using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ILocationService
    {
        void Add (Location location);
        
        Location Get(int locationId);

        IEnumerable<Location> Get(string name = null);

        IEnumerable<string> GetRelated(string name);

        IEnumerable<Location> GetByRegion(int regionId);

        IEnumerable<string> GetForAutocomplete(string prefixText, int count);

        IEnumerable<string> FilterOrigin(IEnumerable<string> locations);
    }
}
