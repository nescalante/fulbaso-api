using System.Linq;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    public class FavouriteService : IFavouriteService
    {
        public void Add(int placeId, long userId)
        {
            if (EntityUtil.Context.UserFavourites.Where(uf => uf.PlaceId == placeId && uf.UserId == userId).Any()) return;

            var favourite = new UserFavourite
            {
                PlaceId = placeId,
                UserId = userId,
            };

            EntityUtil.Context.AddToUserFavourites(favourite);
            EntityUtil.Context.SaveChanges();
        }

        public void Remove(int placeId, long userId)
        {
            var uf = EntityUtil.Context.UserFavourites.FirstOrDefault(o => o.PlaceId == placeId && o.UserId == userId);

            if (uf != null)
            {
                EntityUtil.Context.DeleteObject(uf);
                EntityUtil.Context.SaveChanges();
            }
        }

        public bool IsFavourite(int placeId, long userId)
        {
            return EntityUtil.Context.UserFavourites.Where(uf => uf.PlaceId == placeId && uf.UserId == userId).Any();
        }
    }
}
