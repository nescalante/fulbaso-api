using Fulbaso.Contract;
using Fulbaso.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Fulbaso.EntityFramework.Logic
{
    public class CourtBookLogic : ICourtBookLogic
    {
        private IClientLogic _clientService;
        private ObjectContextEntities _context;

        public CourtBookLogic(ObjectContextEntities context, IClientLogic clientService)
        {
            _clientService = clientService;
            _context = context;
        }

        public void Add(CourtBook book)
        {
            if (book.Client.Id == 0)
            {
                _clientService.Add(book.Client);
            }

            var courtBookEntity = new CourtBookEntity
            {
                CourtId = book.Court.Id,
                StartTime = book.StartTime,
                EndTime = book.EndTime,
                Price = book.Price,
                ReserveRequired = book.ReserveRequired,
                Reserve = book.Reserve,
                Paid = book.Paid,
                ClientId = book.Client.Id,
                Comment = book.Comment,
                UserId = book.User,
            };

            _context.CourtBooks.AddObject(courtBookEntity);
            _context.SaveChanges();

            book.Id = courtBookEntity.Id;
        }

        public void Update(CourtBook book)
        {
            if (book.Client.Id == 0)
            {
                _clientService.Add(book.Client);
            }

            var courtBookEntity = _context.CourtBooks.First(cb => cb.Id == book.Id);

            courtBookEntity = new CourtBookEntity
            {
                Id = book.Id,
                CourtId = book.Court.Id,
                StartTime = book.StartTime,
                EndTime = book.EndTime,
                Price = book.Price,
                ReserveRequired = book.ReserveRequired,
                Reserve = book.Reserve,
                Paid = book.Paid,
                ClientId = book.Client.Id,
                Comment = book.Comment,
                UserId = book.User,
            };

            _context.SaveChanges();
        }

        public void Delete(int courtBookId)
        {
            this.Delete(CourtBook.Create<CourtBook>(courtBookId));
        }

        private void Delete(CourtBook courtBook)
        {
            _context.CourtBooks.DeleteObject(new CourtBookEntity { Id = courtBook.Id });
            _context.SaveChanges();
        }

        public CourtBook Get(int courtBookId)
        {
            return this.Get(r => r.Id == courtBookId).SingleOrDefault();
        }

        public IEnumerable<CourtBook> GetByPlace(int placeId, DateTime startTime, DateTime endTime)
        {
            return this.Get(r => r.Court.PlaceId == placeId && (r.StartTime == null || r.StartTime >= startTime) && (r.EndTime == null || r.EndTime <= endTime));
        }

        public IEnumerable<CourtBook> GetByCourt(int courtId, DateTime startTime, DateTime endTime)
        {
            return this.Get(r => r.CourtId == courtId && (r.StartTime == null || r.StartTime >= startTime) && (r.EndTime == null || r.EndTime <= endTime));
        }

        public IEnumerable<Court> GetAvailable(int[] players, int[] floorTypes, string[] locations, byte[] tags, bool indoor, bool lighted, DateTime date, int hour, int page, int rows, out int count)
        {
            // fix blank entries
            locations = locations.Where(l => !string.IsNullOrEmpty(l)).ToArray();

            var query = from c in _context.Courts
                        where players.Contains(c.Players ?? 0) &&
                        floorTypes.Contains(c.FloorType.Id)
                        select c;
            if (indoor) query = query.Where(c => c.IsIndoor);
            if (lighted) query = query.Where(c => c.IsLighted);
            if (tags.Any())
            {
                var ints = tags.Select(t => Convert.ToInt32(t));
                query = query.Where(c => ints.All(t => c.Place.Services.Any(s => s.Service == t)));
            }

            // filter locations
            query = from c in query
                    where (locations.Count() == 0 || locations.Contains(c.Place.Location.Description) ||
                    locations.Contains(c.Place.Location.Region.Description) ||
                    locations.Contains(c.Place.Location.Region.Territory.Description))
                    select c;

            //filter book availability
            query = query.Where(c => c.Place.Admins.Any());

            //filter court book range
            query = from c in query
                    where c.Configurations.Any(cc => (cc.StartHour <= (hour + 1) || cc.StartHour == null)
                    && (cc.EndHour >= (hour - 1) || cc.EndHour == null)) || !c.Configurations.Any()
                    select c;
            
            // filter hour & date availability
            query = from c in query
                    where !c.Books.Any(b => (b.StartTime.Hour == hour - 1) && (date == null || (b.StartTime.Year == date.Year && b.StartTime.Month == date.Month && b.StartTime.Day == date.Day))) ||
                    !c.Books.Any(b => (b.StartTime.Hour == hour) && (date == null || (b.StartTime.Year == date.Year && b.StartTime.Month == date.Month && b.StartTime.Day == date.Day))) ||
                    !c.Books.Any(b => (b.StartTime.Hour == (hour + 1)) && (date == null || (b.StartTime.Year == date.Year && b.StartTime.Month == date.Month && b.StartTime.Day == date.Day)))
                    select c;

            count = query.Count();

            var courts = query.OrderBy(q => q.Place.Name).Skip((page - 1) * rows).Take(rows);

            return (from c in courts.Include(c => c.Configurations).Include(c => c.Books).Include(c => c.Place).ToList()
                    select new Court
                    {
                        Id = c.Id,
                        Place = new Place
                        {
                            Id = c.PlaceId,
                            Description = c.Place.Name,
                            Page = c.Place.Page,
                        },
                        Description = c.Name,
                        Players = c.Players,
                        CourtType = c.Type.ToEntity<CourtType>(),
                        FloorType = c.FloorType.ToEntity<FloorType>(),
                        IsIndoor = c.IsIndoor,
                        IsLighted = c.IsLighted,
                        IsActive = c.IsActive,
                        Books = CourtBookLogic.Get(c.Books.AsQueryable()),
                        Configuration = CourtConfigurationLogic.Get(c.Configurations.AsQueryable()),
                    }).ToList();
        }

        internal IEnumerable<CourtBook> Get(Expression<Func<CourtBookEntity, bool>> predicate)
        {
            var query = _context.CourtBooks.Where(predicate);
            return CourtBookLogic.Get(query);
        }

        internal static IEnumerable<CourtBook> Get(IQueryable<CourtBookEntity> query)
        {
            return (from r in (query is ObjectQuery<CourtBookEntity> ? query.Include(q => q.Client).Include(q => q.Court) : query).ToList()
                    select new CourtBook
                    {
                        Id = r.Id,
                        Court = Court.Create<Court>(r.CourtId, r.Court.Name, r.Court.IsActive),
                        StartTime = r.StartTime,
                        EndTime = r.EndTime,
                        Price = r.Price,
                        ReserveRequired = r.ReserveRequired,
                        Reserve = r.Reserve,
                        Paid = r.Paid,
                        Client = r.Client != null ? new Client { Id = r.ClientId, Description = r.Client.Name, Phone = r.Client.Phone, Place = Place.Create<Place>(r.Client.PlaceId), IsActive = r.Client.IsActive, } : null,
                        Comment = r.Comment,
                        User = r.UserId,
                    }).ToList();
        }
    }
}
