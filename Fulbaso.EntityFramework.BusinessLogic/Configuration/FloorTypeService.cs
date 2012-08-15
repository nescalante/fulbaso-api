using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class FloorTypeService : IFloorTypeService
    {
        public FloorType Get(int floorTypeId)
        {
            return FloorTypeService.Get(r => r.Id == floorTypeId).SingleOrDefault();
        }

        public IEnumerable<FloorType> Get(string name = null)
        {
            return FloorTypeService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<FloorType> Get(Expression<Func<FloorTypeEntity, bool>> predicate)
        {
            return Repository<FloorTypeEntity>.Get<FloorType>(predicate);
        }
    }
}
