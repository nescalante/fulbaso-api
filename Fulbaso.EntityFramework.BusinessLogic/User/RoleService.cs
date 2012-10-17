using System.Linq;
using Fulbaso.Contract;

namespace Fulbaso.EntityFramework.Logic
{
    public class RoleService : IRoleService
    {
        public bool IsUserInRole(long userId, string role)
        {
            return EntityUtil.Context.UserRoles.Where(ur => ur.UserId == userId && role == ur.Role).ToList().Any();
        }

        public bool IsUserInRole(long userId, int placeId, string role)
        {
            return EntityUtil.Context.UserPlaces.Where(ur => ur.UserId == userId && ur.PlaceId == placeId && role == ur.Role).ToList().Any();
        }

        public void AddRole(long userId, string role)
        {
            var entity = new UserRoleEntity
            {
                UserId = userId,
                Role = role,
            };

            EntityUtil.Context.AddToUserRoles(entity);
            EntityUtil.Context.SaveChanges();
        }

        public void AddRole(long userId, int placeId, string role)
        {
            var entity = new UserPlaceEntity
            {
                UserId = userId,
                PlaceId = placeId,
                Role = role,
            };

            EntityUtil.Context.AddToUserPlaces(entity);
            EntityUtil.Context.SaveChanges();
        }

        public void DeleteRole(long userId, string role)
        {
            var entity = EntityUtil.Context.UserRoles.Where(ur => ur.UserId == userId && ur.Role == role).ToList().First();
            
            EntityUtil.Context.DeleteObject(entity);
            EntityUtil.Context.SaveChanges();
        }

        public void DeleteRole(long userId, int placeId, string role)
        {
            var entity = EntityUtil.Context.UserPlaces.Where(ur => ur.UserId == userId && ur.PlaceId == placeId && ur.Role == role).ToList().First();

            EntityUtil.Context.DeleteObject(entity);
            EntityUtil.Context.SaveChanges();
        }
    }
}
