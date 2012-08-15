using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface IFloorTypeService
    {
        FloorType Get(int floorTypeId);

        IEnumerable<FloorType> Get(string name = null);
    }
}
