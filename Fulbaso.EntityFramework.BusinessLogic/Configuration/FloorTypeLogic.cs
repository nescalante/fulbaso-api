using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class FloorTypeLogic : IFloorTypeLogic
    {
        private ObjectContextEntities _context;

        public FloorTypeLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public FloorType Get(int floorTypeId)
        {
            return this.Get(r => r.Id == floorTypeId).SingleOrDefault();
        }

        public IEnumerable<FloorType> Get(string name = null)
        {
            return this.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal IEnumerable<FloorType> Get(Expression<Func<FloorTypeEntity, bool>> predicate)
        {
            return _context.FloorTypes.Where(predicate).Get<FloorType, FloorTypeEntity>();
        }
    }
}
