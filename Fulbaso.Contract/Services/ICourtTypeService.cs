using System.Collections.Generic;
using Fulbaso.Common;

namespace Fulbaso.Contract
{
    public interface ICourtTypeService
    {
        EntityDataObject Get(int courtTypeId);

        IEnumerable<EntityDataObject> Get(string name = null);
    }
}
