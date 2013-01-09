using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ITerritoryLogic
    {
        void Add(Territory territory);

        Territory Get(int territoryId);

        IEnumerable<Territory> Get(string name = null);
    }
}
