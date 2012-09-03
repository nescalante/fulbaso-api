using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface ITerritoryService
    {
        void Add(Territory territory);

        Territory Get(int territoryId);

        IEnumerable<Territory> Get(string name = null);
    }
}
