using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface ITerritoryService
    {
        Territory Get(int territoryId);

        IEnumerable<Territory> Get(string name = null);
    }
}
