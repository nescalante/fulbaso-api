using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class TerritoryService : ITerritoryService
    {
        public EntityDataObject Get(int territoryId)
        {
            return TerritoryService.Get(r => r.Id == territoryId).SingleOrDefault();
        }

        public IEnumerable<EntityDataObject> Get(string name = null)
        {
            return TerritoryService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<EntityDataObject> Get(Expression<Func<TerritoryEntity, bool>> predicate)
        {
            return Repository<TerritoryEntity>.Get(predicate);
        }
    }
}
