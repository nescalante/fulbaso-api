using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    public class FloorTypeService : IFloorTypeService
    {
        public EntityDataObject Get(int floorTypeId)
        {
            return FloorTypeService.Get(r => r.Id == floorTypeId).Single();
        }

        public IEnumerable<EntityDataObject> Get(string name = null)
        {
            return FloorTypeService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<EntityDataObject> Get(Expression<Func<FloorTypeEntity, bool>> predicate)
        {
            return Repository<FloorTypeEntity>.Get(predicate);
        }
    }
}
