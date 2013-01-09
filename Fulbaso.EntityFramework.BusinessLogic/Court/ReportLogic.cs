using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.Logic
{
    public class ReportLogic : IReportLogic
    {
        private ObjectContextEntities _context;

        public ReportLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public IEnumerable<Place> GetTopUsedPlaces(int count)
        {
            var time = DateTime.Now.AddMonths(-1);
            var query = _context.Places.OrderByDescending(p => 
                p.Courts.SelectMany(c => c.Books.Where(b => b.StartTime > time)).Count());

            return PlaceLogic.Get(query.Take(count));
        }

        public IEnumerable<Place> GetTopVotedPlaces(int count)
        {
            var query = _context.Places.OrderByDescending(p => p.Favourites.Count());

            return PlaceLogic.Get(query.Take(count));
        }

        public int GetPlacesCount()
        {
            return _context.Places.Count();
        }

        public int GetCourtsCount()
        {
            return _context.Courts.Count();
        }

        public int GetOwnedPlaces()
        {
            return _context.UserPlaces.Select(u => u.PlaceId).Distinct().Count();
        }
    }
}   
