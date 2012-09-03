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
        public void Add(Territory territory)
        {
            var entity = new TerritoryEntity
            {
                Description = territory.Description,
                IsActive = true,
            };

            EntityUtil.Context.Territories.AddObject(entity);
            EntityUtil.Context.SaveChanges();

            territory.Id = entity.Id;
        }

        public Territory Get(int territoryId)
        {
            return TerritoryService.Get(r => r.Id == territoryId).SingleOrDefault();
        }

        public IEnumerable<Territory> Get(string name = null)
        {
            return TerritoryService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<Territory> Get(Expression<Func<TerritoryEntity, bool>> predicate)
        {
            return Repository<TerritoryEntity>.Get<Territory>(predicate);
        }
    }
}
