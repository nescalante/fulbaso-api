using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class LocationService : ILocationService
    {
        public void Add(Location location)
        {
            var entity = new LocationEntity
            {
                Description = location.Description,
                RegionId = location.Region.Id,
                IsActive = true,
            };

            EntityUtil.Context.Locations.AddObject(entity);
            EntityUtil.Context.SaveChanges();

            location.Id = entity.Id;
        }

        public Location Get(int locationId)
        {
            return LocationService.Get(r => r.Id == locationId).SingleOrDefault();
        }

        public IEnumerable<Location> Get(string name = null)
        {
            return LocationService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        public IEnumerable<string> GetRelated(string name)
        {
            return EntityUtil.Context.RelatedLocations.Where(l => l.Related == name).Select(l => l.Value).ToList();
        }

        public IEnumerable<Location> GetByRegion(int regionId)
        {
            return LocationService.Get(r => r.RegionId == regionId).ToList();
        }

        public IEnumerable<string> GetForAutocomplete(string prefixText, int count)
        {
            var comparer = new CaseInsensitiveComparer();

            var remaining = count;

            var locations = EntityUtil.Context.Locations.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            var list = locations.OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            if (locations.Count() == remaining) return list;

            remaining -= locations.Count();
            var regions = EntityUtil.Context.Regions.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            list = list.Concat(regions).OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            return list;
        }

        internal static IEnumerable<Location> Get(Expression<Func<LocationEntity, bool>> predicate)
        {
            var query = Repository<LocationEntity>.GetQuery(predicate);
            return LocationService.Get(query);
        }

        internal static IEnumerable<Location> Get(IQueryable<LocationEntity> query)
        {
            return (from r in query.Include(p => p.Region).ToList()
                    select new Location
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Region = r.Region.ToEntity<Region>(),
                        IsActive = r.IsActive,
                    }).ToList();
        }
    }
}
