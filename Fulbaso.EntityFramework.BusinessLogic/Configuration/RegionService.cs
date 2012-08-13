using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class RegionService : IRegionService
    {
        public Region Get(int regionId)
        {
            return RegionService.Get(r => r.Id == regionId).SingleOrDefault();
        }

        public IEnumerable<Region> Get(string name = null)
        {
            return RegionService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<Region> Get(Expression<Func<RegionEntity, bool>> predicate)
        {
            var query = Repository<RegionEntity>.GetQuery(predicate);
            return RegionService.Get(query);
        }

        internal static IEnumerable<Region> Get(IQueryable<RegionEntity> query)
        {
            return (from r in query.Include(q => q.Territory).ToList()
                    select new Region
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Territory = r.Territory.ToEntity(),
                        IsActive = r.IsActive,
                    }).ToList();
        }
    }
}
