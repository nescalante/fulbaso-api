using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface IFloorTypeService
    {
        EntityDataObject Get(int floorTypeId);

        IEnumerable<EntityDataObject> Get(string name = null);
    }
}
