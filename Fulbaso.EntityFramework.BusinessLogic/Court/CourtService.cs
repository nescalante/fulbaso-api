using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    public class CourtService : ICourtService
    {
        public void Add(Court court)
        {
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

            Repository<CourtEntity>.Add(courtEntity);
            court.Id = courtEntity.Id;
        }

        public void Update(Court court)
        {
            var courtEntity = EntityUtil.Context.Courts.Where(c => c.Id == court.Id).ToList().First();

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

            EntityUtil.Context.ApplyCurrentValues(EntityUtil.Context.Courts.EntitySet.Name, courtEntity);
            EntityUtil.Context.SaveChanges();
        }

        public void Delete(int courtId)
        {
            CourtService.Delete(Court.Create<Court>(courtId));
        }

        private static void Delete(Court court)
        {
            Repository<CourtEntity>.Delete(new CourtEntity { Id = court.Id });
        }

        public Court Get(int courtId)
        {
            return CourtService.Get(r => r.Id == courtId).SingleOrDefault();
        }

        public IEnumerable<Court> GetByPlace(int placeId)
        {
            return CourtService.Get(c => c.PlaceId == placeId);
        }

        internal static IEnumerable<Court> Get(Expression<Func<CourtEntity, bool>> predicate)
        {
            var query = Repository<CourtEntity>.GetQuery(predicate);
            return CourtService.Get(query);
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
                        CourtType = r.Type.ToEntity(),
                        FloorType = r.FloorType.ToEntity(),
                        IsIndoor = r.IsIndoor,
                        IsLighted = r.IsLighted,
                        IsActive = r.IsActive,
                    }).ToList();
        }
    }
}
