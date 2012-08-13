using System.Linq;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.Logic
{
    public class FavouriteService : IFavouriteService
    {
        public void Add(int placeId, long userId)
        {
            if (this.IsFavourite(placeId, userId)) return;

            var place = EntityUtil.Context.Places.Where(p => p.Id == placeId).ToList().First();
            var user = EntityUtil.Context.Users.Where(u => u.Id == userId).ToList().First();

            user.Favourites.Add(place);

            EntityUtil.Context.SaveChanges();
        }

        public void Remove(int placeId, long userId)
        {
            if (!this.IsFavourite(placeId, userId)) return;

            var place = EntityUtil.Context.Places.Where(p => p.Id == placeId).ToList().First();
            var user = EntityUtil.Context.Users.Where(u => u.Id == userId).ToList().First();

            user.Favourites.Remove(place);

            EntityUtil.Context.SaveChanges();
        }

        public bool IsFavourite(int placeId, long userId)
        {
            return EntityUtil.Context.Users.Any(u => u.Favourites.Any(uf => uf.Id == placeId) && u.Id == userId);
        }
    }
}
