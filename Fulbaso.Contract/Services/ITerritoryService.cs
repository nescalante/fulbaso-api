using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface ITerritoryService
    {
        EntityDataObject Get(int territoryId);

        IEnumerable<EntityDataObject> Get(string name = null);
    }
}
