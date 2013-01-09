using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class LocationLogic : ILocationLogic
    {
        private ITerritoryLogic _territoryService;
        private IRegionLogic _regionService;
        private ObjectContextEntities _context;

        public LocationLogic(ObjectContextEntities context, ITerritoryLogic territoryService, IRegionLogic regionService)
        {
            _territoryService = territoryService;
            _regionService = regionService;
            _context = context;
        }

        public void Add(Location location)
        {
            var entity = new LocationEntity
            {
                Description = location.Description,
                RegionId = location.Region.Id,
                IsActive = true,
            };

            _context.Locations.AddObject(entity);
            _context.SaveChanges();

            location.Id = entity.Id;
        }

        public Location Get(int locationId)
        {
            return this.Get(r => r.Id == locationId).SingleOrDefault();
        }

        public IEnumerable<Location> Get(string name = null)
        {
            return this.Get(c => string.IsNullOrEmpty(name) || c.Description == name);
        }

        public IEnumerable<string> GetRelated(string name)
        {
            return _context.RelatedLocations.Where(l => l.Related == name).Select(l => l.Value).ToList();
        }

        public IEnumerable<Location> GetByRegion(int regionId)
        {
            return this.Get(r => r.RegionId == regionId).ToList();
        }

        public IEnumerable<string> GetForAutocomplete(string prefixText, int count)
        {
            var comparer = new CaseInsensitiveComparer();

            var remaining = count;

            var locations = _context.Locations.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            var list = locations.OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            if (locations.Count() == remaining) return list;

            remaining -= locations.Count();
            var regions = _context.Regions.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            list = list.Concat(regions).OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            return list;
        }

        internal IEnumerable<Location> Get(Expression<Func<LocationEntity, bool>> predicate)
        {
            var query = _context.Locations.Where(predicate);
            return LocationLogic.Get(query);
        }

        internal static IEnumerable<Location> Get(IQueryable<LocationEntity> query)
        {
            return (from r in query.ToList()
                    select new Location
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Region = EntityDataObject.Create<Region>(r.RegionId),
                        IsActive = r.IsActive,
                    }).ToList();
        }

        public IEnumerable<string> FilterOrigin(IEnumerable<string> locations)
        {
            locations = locations.Distinct(new CaseInsensitiveComparer());

            var entities = this.GetEntities(locations);
            var list = new List<string>();

            foreach (var e in entities.Where(e => e != null))
            {
                if (!((e is Territory && entities.Where(r => r is Region && (r as Region).Territory.Id == e.Id).Any()) || 
                    (e is Region && entities.Where(r => r is Location && (r as Location).Region.Id == e.Id).Any())))
                {
                    list.Add(e.Description);
                }
            }

            return list.Distinct().ToList();
        }

        private IEnumerable<IEntity> GetEntities(IEnumerable<string> locations)
        {
            return (from l in locations
                    let lo = this.Get(l).FirstOrDefault()
                    let ro = _regionService.Get(l).FirstOrDefault()
                    let to = _territoryService.Get(l).FirstOrDefault()
                    select to != null ? to as IEntity:
                           ro != null ? ro as IEntity:
                           lo != null ? lo as IEntity:
                           null).ToList();
        }
    }
}
