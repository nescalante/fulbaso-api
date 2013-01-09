using System.Linq;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.Logic
{
    public class FavouriteLogic : IFavouriteLogic
    {
        private ObjectContextEntities _context;

        public FavouriteLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public void Add(int placeId, long userId)
        {
            if (this.IsFavourite(placeId, userId)) return;

            var place = _context.Places.Where(p => p.Id == placeId).ToList().First();
            var user = _context.Users.Where(u => u.Id == userId).ToList().First();

            user.Favourites.Add(place);

            _context.SaveChanges();
        }

        public void Remove(int placeId, long userId)
        {
            if (!this.IsFavourite(placeId, userId)) return;

            var place = _context.Places.Where(p => p.Id == placeId).ToList().First();
            var user = _context.Users.Where(u => u.Id == userId).ToList().First();

            user.Favourites.Remove(place);

            _context.SaveChanges();
        }

        public bool IsFavourite(int placeId, long userId)
        {
            return _context.Users.Any(u => u.Favourites.Any(uf => uf.Id == placeId) && u.Id == userId);
        }
    }
}
