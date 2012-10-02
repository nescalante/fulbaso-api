using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.Helpers;
using File = Fulbaso.Contract.File;

namespace Fulbaso.EntityFramework.Logic
{
    public class PlaceService : IPlaceService
    {
        private ICourtBookService _bookService;
        private IFileService _fileService;

        public PlaceService(ICourtBookService bookService, IFileService fileService)
        {
            _bookService = bookService;
            _fileService = fileService;
        }

        public void Add(Place place)
        {
            var placeEntity = new PlaceEntity
            {
                Name = place.Description,
                Description = place.Info,
                Address = place.Address,
                LocationId = place.Location.Id,
                MapLocation = place.MapLocation,
                MapUa = place.MapUa,
                MapVa = place.MapVa,
                Phone = place.Phone,
                HowToArrive = place.HowToArrive,
                DateFrom = DateTime.Today,
                IsActive = place.IsActive,
                Page = place.Page,
            };

            foreach (var s in place.Services)
                placeEntity.Services.Add(new Fulbaso.EntityFramework.PlaceService { Service = (byte)s });

            Repository<PlaceEntity>.Add(placeEntity);
            place.Id = placeEntity.Id;
        }

        public void Update(Place place)
        {
            var entity = EntityUtil.Context.Places.Where(p => p.Id == place.Id).ToList().First();

            entity.Name = place.Description;
            entity.Description = place.Info;
            entity.Address = place.Address;
            entity.LocationId = place.Location.Id;
            entity.MapLocation = place.MapLocation;
            entity.MapUa = place.MapUa;
            entity.MapVa = place.MapVa;
            entity.Phone = place.Phone;
            entity.HowToArrive = place.HowToArrive;
            entity.Page = place.Page;

            while (entity.Services.Any()) EntityUtil.Context.DeleteObject(entity.Services.First());
            EntityUtil.Context.SaveChanges();

            place.Services.ToList().ForEach(s => entity.Services.Add(new Fulbaso.EntityFramework.PlaceService { Service = (byte)s }));
            EntityUtil.Context.SaveChanges();
        }

        public void Delete(int placeId)
        {
            PlaceService.Delete(Place.Create<Place>(placeId));
        }

        private static void Delete(Place place)
        {
            Repository<PlaceEntity>.Delete(new PlaceEntity { Id = place.Id });
        }

        public void AddImage(int placeId, Stream input, File file)
        {
            _fileService.AddImage(input, file);

            var placeEntity = EntityUtil.Context.Places.Where(p => p.Id == placeId).ToList().First();
            var fileEntity = EntityUtil.Context.Files.Where(f => f.Id == file.Id).ToList().First();

            placeEntity.Files.Add(fileEntity);

            EntityUtil.Context.SaveChanges();
        }

        public Place Get(int placeId)
        {
            return PlaceService.Get(r => r.Id == placeId).SingleOrDefault();
        }

        public IEnumerable<string> GetForAutocomplete(string prefixText, int count)
        {
            return AutocompleteService.GetForAutocomplete(prefixText, count);
        }

        public IEnumerable<Tuple<string, int>> GetTags()
        {
            return AutocompleteService.GetTags();
        }

        public IEnumerable<Place> GetByUser(long userId)
        {
            var query = EntityUtil.Context.Users.Where(u => u.Id == userId)
                .SelectMany(u => u.Places)
                .Select(p => new Place { Description = p.Name, Page = p.Page, Id = p.Id, });

            return query.ToList();
        }

        public IEnumerable<Place> GetList(string value, decimal? latitude, decimal? longitude, int init, int rows, out int count)
        {
            var query = EntityUtil.Context.PlaceViews.Where(c => string.IsNullOrEmpty(value) ||
                c.exp.Contains(value));
            count = query.Count();

            // order by distance
            if (string.IsNullOrEmpty(value) && latitude.HasValue && longitude.HasValue)
            {
                query = PlaceService.OrderByDistance(query, latitude.Value, longitude.Value);
            }
            else
            {
                query = query.OrderBy(q => q.exp.ToUpper().IndexOf(value.ToUpper()));
            }

            query = query.Skip(init);

            if (rows > 0) query = query.Take(rows);

            return GetFromView(query);
        }

        private List<Place> GetFromView(IQueryable<PlaceView> query)
        {
            var list = (from i in query.ToList()
                        select new Place
                        {
                            Id = i.Id,
                            Description = i.Name,
                            Address = i.Address,
                            Phone = i.Phone,
                            Page = i.Page,
                            Location = new Location { Description = i.Location, Region = Region.Create<Region>(i.Region), },
                            Courts = (int)i.Courts,
                            MapUa = i.MapUa,
                            MapVa = i.MapVa,
                        }).ToList();

            var services = EntityUtil.Context.PlaceServices.WhereContains(p => p.PlaceId, list.Select(i => i.Id)).ToList();
            list.ForEach(i => i.Services = services.Where(s => i.Id == s.PlaceId).Select(s => (Service)s.Service));

            var images = EntityUtil.Context.Places.WhereContains(p => p.Id, list.Select(i => i.Id)).Select(p => new { p.Files, p.Id }).ToList();
            list.ForEach(i => i.Images = images.Where(s => i.Id == s.Id).First().Files.Select(f => new File
            {
                Id = f.Id,
                ContentLength = f.ContentLength,
                ContentType = f.ContentType,
                Description = f.Description,
                FileName = f.FileName,
                CreatedBy = new User { Id = f.UserId, },
                InsertDate = f.InsertDate,
            }));

            return list;
        }

        public IEnumerable<Place> GetList(string value, decimal? latitude, decimal? longitude, int[] players, int[] floorTypes, string[] locations, byte[] tags, bool indoor, bool lighted, int init, int rows, out int count)
        {
            var places = CreateQuery(latitude, longitude, players, floorTypes, locations, tags, indoor, lighted);

            if (!string.IsNullOrEmpty(value))
            {
                var query = EntityUtil.Context.PlaceViews.Where(c => c.exp.Contains(value)).Select(p => p.Id);
                places = places.WhereContains(p => p.Id, query);
            }

            count = places.Count();
            places = places.OrderBy(q => q.Name).Skip(init);

            if (rows > 0) places = places.Take(rows);

            return PlaceService.Get(places);
        }

        private static IQueryable<PlaceEntity> CreateQuery(decimal? latitude, decimal? longitude, int[] players, int[] floorTypes, string[] locations, byte[] tags, bool indoor, bool lighted)
        {
            // fix blank entries
            locations = locations.Where(l => !string.IsNullOrEmpty(l)).ToArray();

            var query = from c in EntityUtil.Context.Courts
                        where (!players.Any() || players.Contains(c.Players ?? 0)) &&
                        (!floorTypes.Any() || floorTypes.Contains(c.FloorType.Id))
                        select c;
            if (indoor) query = query.Where(c => c.IsIndoor);
            if (lighted) query = query.Where(c => c.IsLighted);

            var places = query.GroupBy(c => c.Place.Id).Select(p => p.FirstOrDefault().Place);
            if (tags.Any())
            {
                var ints = tags.Select(t => Convert.ToInt32(t));
                places = places.Where(p => ints.All(t => p.Services.Any(s => s.Service == t)));
            }

            places = from p in places
                     where (locations.Count() == 0 || locations.Contains(p.Location.Description) ||
                     locations.Contains(p.Location.Region.Description) ||
                     locations.Contains(p.Location.Region.Territory.Description))
                     select p;

            // order by distance
            if (latitude.HasValue && longitude.HasValue)
            {
                places = PlaceService.OrderByDistance(places, latitude.Value, longitude.Value);
            }

            return places;
        }

        public IEnumerable<Place> GetList(string name)
        {
            return PlaceService.Get(c => string.IsNullOrEmpty(name) || c.Name.Contains(name));
        }

        public Place Get(string page)
        {
            var place = PlaceService.Get(r => r.Page.ToLower() == page.ToLower()).SingleOrDefault();
            int placeId;
            bool parsed = int.TryParse(page, out placeId);

            if (place == null && parsed)
            {
                place = this.Get(placeId);
            }

            return place;
        }

        public string ValidatePage(string page, out int id)
        {
            var place = EntityUtil.Context.Places.Where(r => r.Page.ToLower() == page.ToLower()).Select(p => new { p.Page, p.Id, }).ToList().FirstOrDefault();

            int placeId;
            bool parsed = int.TryParse(page, out placeId);

            if (string.IsNullOrEmpty(place.Page) && parsed)
            {
                place = EntityUtil.Context.Places.Where(r => r.Id == placeId).Select(p => new { p.Page, p.Id }).ToList().FirstOrDefault();
            }

            id = place.Id;
            return place.Page;
        }

        public string ValidatePage(string page)
        {
            int id;
            return ValidatePage(page, out id);
        }

        internal static IQueryable<PlaceEntity> OrderByDistance(IQueryable<PlaceEntity> query, decimal lat, decimal lng)
        {
            var pi = (decimal)Math.PI;

            return from p in query.Where(i => i.MapUa != null && i.MapVa != null)
                   let la1 = (decimal)p.MapUa
                   let lo1 = (decimal)p.MapVa
                   let la2 = (decimal)lat
                   let lo2 = (decimal)lng
                   let dist = (SqlFunctions.Acos(SqlFunctions.Sin(la1 * pi / 180M) * SqlFunctions.Sin(la2 * pi / 180M) + SqlFunctions.Cos(la1 * pi / 180M) * SqlFunctions.Cos(la2 * pi / 180M) * SqlFunctions.Cos((lo1 - lo2) * pi / 180M)) / Math.PI * 180)
                   orderby dist
                   select p;
        }

        internal static IQueryable<PlaceView> OrderByDistance(IQueryable<PlaceView> query, decimal lat, decimal lng)
        {
            var pi = (decimal)Math.PI;

            return from p in query.Where(i => i.MapUa != null && i.MapVa != null)
                   let la1 = (decimal)p.MapUa
                   let lo1 = (decimal)p.MapVa
                   let la2 = (decimal)lat
                   let lo2 = (decimal)lng
                   let dist = (SqlFunctions.Acos(SqlFunctions.Sin(la1 * pi / 180M) * SqlFunctions.Sin(la2 * pi / 180M) + SqlFunctions.Cos(la1 * pi / 180M) * SqlFunctions.Cos(la2 * pi / 180M) * SqlFunctions.Cos((lo1 - lo2) * pi / 180M)) / Math.PI * 180)
                   orderby dist
                   select p;
        }

        public IEnumerable<Tuple<Place, double?>> GetNearest(Place place, int count = 10, double distance = 0)
        {
            return GetNearest(place.MapUa, place.MapVa, count, distance);
        }

        public IEnumerable<Tuple<Place, double?>> GetNearest(string place, int count = 10, double distance = 0)
        {
            var page = ValidatePage(place);

            if (string.IsNullOrEmpty(page)) return new List<Tuple<Place, double?>>();

            var entity = EntityUtil.Context.Places.Where(i => i.Page == page).ToList().First();

            return GetNearest(entity.MapUa, entity.MapVa, count, distance);
        }

        public IEnumerable<Tuple<Place, double?>> GetNearest(decimal? lat, decimal? lng, int count = 10, double distance = 0)
        {
            var pi = (decimal)Math.PI;

            if (lat.HasValue && lng.HasValue)
            {
                var near = from p in EntityUtil.Context.Places.Where(i => i.MapUa != null && i.MapVa != null && i.MapUa != lat && i.MapVa != lng)
                           let la1 = (decimal)p.MapUa
                           let lo1 = (decimal)p.MapVa
                           let la2 = (decimal)lat
                           let lo2 = (decimal)lng
                           let dist = (SqlFunctions.Acos(SqlFunctions.Sin(la1 * pi / 180M) * SqlFunctions.Sin(la2 * pi / 180M) + SqlFunctions.Cos(la1 * pi / 180M) * SqlFunctions.Cos(la2 * pi / 180M) * SqlFunctions.Cos((lo1 - lo2) * pi / 180M)) / Math.PI * 180) * 60.0 * 1.1515 * 1.609344
                           orderby dist
                           select new { 
                               Place = new Place 
                               { 
                                   Id = p.Id, 
                                   Description = p.Name, 
                                   Page = p.Page,
                                   Address = p.Address,
                                   Phone = p.Phone,
                                   Location = new Location { Description = p.Location.Description, Region = new Region { Description = p.Location.Region.Description }, },
                                   Courts = p.Courts.Count(),
                                   MapUa = p.MapUa, 
                                   MapVa = p.MapVa, 
                               }, 
                               Distance = dist 
                           };

                if (distance != 0) near = near.Where(p => p.Distance < distance);
                if (count != 0) near = near.Take(count);

                var list = near.ToList();

                var services = EntityUtil.Context.PlaceServices.WhereContains(p => p.PlaceId, list.Select(i => i.Place.Id)).ToList();
                list.ForEach(i => i.Place.Services = services.Where(s => i.Place.Id == s.PlaceId).Select(s => (Service)s.Service));

                return list.ToList().Select(p => new Tuple<Place, double?>(p.Place, p.Distance)).ToList();
            }

            return new List<Tuple<Place, double?>>();
        }

        public Place Get(string page, DateTime day)
        {
            var place = PlaceService.Get(r => r.Page.ToLower() == page.ToLower()).SingleOrDefault();
            int placeId;
            if (place == null && int.TryParse(page, out placeId))
            {
                place = this.Get(placeId);
            }

            if (place == null) return null;

            place.CourtsInfo = CourtService.Get(EntityUtil.Context.Courts.Where(c => c.PlaceId == place.Id));

            var configs = EntityUtil.Context.CourtConfigurations.Where(c => c.Court.PlaceId == place.Id).ToList();
            var books = _bookService.GetByPlace(place.Id, day, day.AddDays(1).AddMilliseconds(-1));
                
            foreach (var c in place.CourtsInfo)
            {
                c.Books = books.Where(cb => cb.Court.Id == c.Id).ToList();
                c.Configuration = CourtConfigurationService.Get(configs.Where(cc => cc.CourtId == c.Id).ToList());
            }

            return place;
        }

        public Place Get(Place place, DateTime day)
        {
            place.CourtsInfo = CourtService.Get(EntityUtil.Context.Courts.Where(c => c.PlaceId == place.Id));
            
            var configs = EntityUtil.Context.CourtConfigurations.Where(c => c.Court.PlaceId == place.Id).ToList();
            var books = _bookService.GetByPlace(place.Id, day, day.AddDays(1).AddMilliseconds(-1));

            foreach (var c in place.CourtsInfo)
            {
                c.Books = books.Where(cb => cb.Court.Id == c.Id).ToList();
                c.Configuration = CourtConfigurationService.Get(configs.Where(cc => cc.CourtId == c.Id).ToList());
            }

            return place;
        }

        internal static IEnumerable<Place> Get(Expression<Func<PlaceEntity, bool>> predicate)
        {
            var query = Repository<PlaceEntity>.GetQuery(predicate);
            return PlaceService.Get(query);
        }

        internal static IEnumerable<Place> Get(IQueryable<PlaceEntity> query)
        {
            return (from r in query.Include(i => i.Services).Include(i => i.Location).Include("Location.Region").Include(i => i.Files).ToList()
                    select new Place
                    {
                        Id = r.Id,
                        Description = r.Name,
                        Info = r.Description,
                        Address = r.Address,
                        Location = r.LocationId != null ? new Location
                        {
                            Id = (int)r.LocationId,
                            Description = r.Location.Description,
                            IsActive = r.IsActive,
                            Region = r.Location.Region.ToEntity<Region>(),
                        } : null,
                        MapLocation = r.MapLocation,
                        MapUa = r.MapUa,
                        MapVa = r.MapVa,
                        Phone = r.Phone,
                        HowToArrive = r.HowToArrive,
                        Services = r.Services.Select(s => (Service)s.Service),
                        Images = r.Files.Select(f => new File
                        {
                            Id = f.Id,
                            ContentLength = f.ContentLength,
                            ContentType = f.ContentType,
                            Description = f.Description,
                            FileName = f.FileName,
                            CreatedBy = new User { Id = f.UserId, },
                            InsertDate = f.InsertDate,
                        }),
                        Page = r.Page,
                        IsActive = r.IsActive,
                        Courts = r.Courts.Count(),
                    }).ToList();
        }

        internal static IQueryable<PlaceEntity> GetPlacesOrderedByLocation(decimal lat, decimal lng)
        {
            var pi = (decimal)Math.PI;

            return from p in EntityUtil.Context.Places.Where(i => i.MapUa != null && i.MapVa != null && i.MapUa != lat && i.MapVa != lng)
                   let la1 = (decimal)p.MapUa
                   let lo1 = (decimal)p.MapVa
                   let la2 = (decimal)lat
                   let lo2 = (decimal)lng
                   let dist = (SqlFunctions.Acos(SqlFunctions.Sin(la1 * pi / 180M) * SqlFunctions.Sin(la2 * pi / 180M) + SqlFunctions.Cos(la1 * pi / 180M) * SqlFunctions.Cos(la2 * pi / 180M) * SqlFunctions.Cos((lo1 - lo2) * pi / 180M)) / Math.PI * 180) * 60.0 * 1.1515 * 1.609344
                   orderby dist
                   select p;
        }

        public bool PlaceHasAdmin(int placeId)
        {
            return EntityUtil.Context.Places.Where(p => p.Id == placeId && p.Admins.Any()).Any();
        }
    }
}
