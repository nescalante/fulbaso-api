using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class CourtLogic : ICourtLogic
    {
        private ObjectContextEntities _context;

        public CourtLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public void Add(Court court)
        {
            if (string.IsNullOrEmpty(court.Description))
            {
                var number = _context.Courts.Where(c => c.PlaceId == court.Place.Id).Count() + 1;
                court.Description = "Cancha " + number;
            }

            var courtEntity = new CourtEntity
            {
                PlaceId = court.Place.Id,
                Name = court.Description,
                Players = court.Players,
                CourtTypeId = Convert.ToByte(court.CourtType.Id),
                FloorTypeId = Convert.ToByte(court.FloorType.Id),
                IsIndoor = court.IsIndoor,
                IsLighted = court.IsLighted,
                IsActive = court.IsActive,
            };

            _context.Courts.AddObject(courtEntity);
            _context.SaveChanges();

            court.Id = courtEntity.Id;
        }

        public void Update(Court court)
        {
             court.Description = court.Description ?? "";

            var courtEntity = _context.Courts.Where(c => c.Id == court.Id).ToList().First();

            courtEntity = new CourtEntity
            {
                Id = court.Id,
                PlaceId = court.Place.Id,
                Name = court.Description,
                Players = court.Players,
                CourtTypeId = Convert.ToByte(court.CourtType.Id),
                FloorTypeId = Convert.ToByte(court.FloorType.Id),
                IsIndoor = court.IsIndoor,
                IsLighted = court.IsLighted,
                IsActive = court.IsActive,
            };

            _context.ApplyCurrentValues(_context.Courts.EntitySet.Name, courtEntity);
            _context.SaveChanges();
        }

        public void Delete(int courtId)
        {
            this.Delete(Court.Create<Court>(courtId));
        }

        private void Delete(Court court)
        {
            _context.Courts.DeleteObject(new CourtEntity { Id = court.Id });
            _context.SaveChanges();
        }

        public Court Get(int courtId)
        {
            return this.Get(r => r.Id == courtId).SingleOrDefault();
        }

        public IEnumerable<Court> GetByPlace(int placeId)
        {
            return this.Get(c => c.PlaceId == placeId);
        }

        internal IEnumerable<Court> Get(Expression<Func<CourtEntity, bool>> predicate)
        {
            var query = _context.Courts.Where(predicate);
            return CourtLogic.Get(query);
        }

        internal static IEnumerable<Court> Get(IQueryable<CourtEntity> query)
        {
            return (from r in query.Include(p => p.Place)
                        .Include(p => p.Type)
                        .Include(p => p.FloorType).OrderBy(p => p.Name).ToList()
                    select new Court
                    {
                        Id = r.Id,
                        Place = new Place {
                            Id = r.PlaceId,
                            Description = r.Place.Name,
                        },
                        Description = r.Name,
                        Players = r.Players,
                        CourtType = r.Type.ToEntity<CourtType>(),
                        FloorType = r.FloorType.ToEntity<FloorType>(),
                        IsIndoor = r.IsIndoor,
                        IsLighted = r.IsLighted,
                        IsActive = r.IsActive,
                    }).ToList();
        }
    }
}
