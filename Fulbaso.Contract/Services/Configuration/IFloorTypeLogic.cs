using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IFloorTypeLogic
    {
        FloorType Get(int floorTypeId);

        IEnumerable<FloorType> Get(string name = null);
    }
}
