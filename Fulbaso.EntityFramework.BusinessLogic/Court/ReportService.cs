using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.Logic
{
    public class ReportService : IReportService
    {
        public IEnumerable<Place> GetTopUsedPlaces(int count)
        {
            var time = DateTime.Now.AddMonths(-1);
            var query = EntityUtil.Context.Places.OrderByDescending(p => 
                p.Courts.SelectMany(c => c.Books.Where(b => b.StartTime > time)).Count());

            return PlaceService.Get(query.Take(count));
        }

        public IEnumerable<Place> GetTopVotedPlaces(int count)
        {
            var query = EntityUtil.Context.Places.OrderByDescending(p => p.Favourites.Count());

            return PlaceService.Get(query.Take(count));
        }

        public int GetPlacesCount()
        {
            return EntityUtil.Context.Places.Count();
        }

        public int GetCourtsCount()
        {
            return EntityUtil.Context.Courts.Count();
        }

        public int GetOwnedPlaces()
        {
            return EntityUtil.Context.Users.SelectMany(u => u.Places.Select(up => up.Id)).Distinct().Count();
        }
    }
}   
