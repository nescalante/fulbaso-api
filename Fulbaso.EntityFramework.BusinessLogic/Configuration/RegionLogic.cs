using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class RegionLogic : IRegionLogic
    {
        private ObjectContextEntities _context;

        public RegionLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public void Add(Region region)
        {
            var entity = new RegionEntity
            {
                Description = region.Description,
                TerritoryId = region.Territory.Id,
                IsActive = true,
            };

            _context.Regions.AddObject(entity);
            _context.SaveChanges();

            region.Id = entity.Id;
        }

        public Region Get(int regionId)
        {
            return this.Get(r => r.Id == regionId).SingleOrDefault();
        }

        public IEnumerable<Region> Get(string name = null)
        {
            return this.Get(c => string.IsNullOrEmpty(name) || c.Description == name);
        }

        internal IEnumerable<Region> Get(Expression<Func<RegionEntity, bool>> predicate)
        {
            var query = _context.Regions.Where(predicate);
            return RegionLogic.Get(query);
        }

        internal static IEnumerable<Region> Get(IQueryable<RegionEntity> query)
        {
            return (from r in query.ToList()
                    select new Region
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Territory = EntityDataObject.Create<Region>(r.TerritoryId),
                        IsActive = r.IsActive,
                    }).ToList();
        }
    }
}
