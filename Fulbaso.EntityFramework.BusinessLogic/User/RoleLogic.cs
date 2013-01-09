using System.Linq;
using Fulbaso.Contract;

namespace Fulbaso.EntityFramework.Logic
{
    public class RoleLogic : IRoleLogic
    {
        private ObjectContextEntities _context;

        public RoleLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public bool IsUserInRole(long userId, string role)
        {
            return _context.UserRoles.Where(ur => ur.UserId == userId && role == ur.Role).ToList().Any();
        }

        public bool IsUserInRole(long userId, int placeId, string role)
        {
            return _context.UserPlaces.Where(ur => ur.UserId == userId && ur.PlaceId == placeId && role == ur.Role).ToList().Any();
        }

        public void AddRole(long userId, string role)
        {
            var entity = new UserRoleEntity
            {
                UserId = userId,
                Role = role,
            };

            _context.AddToUserRoles(entity);
            _context.SaveChanges();
        }

        public void AddRole(long userId, int placeId, string role)
        {
            var entity = new UserPlaceEntity
            {
                UserId = userId,
                PlaceId = placeId,
                Role = role,
            };

            _context.AddToUserPlaces(entity);
            _context.SaveChanges();
        }

        public void DeleteRole(long userId, string role)
        {
            var entity = _context.UserRoles.Where(ur => ur.UserId == userId && ur.Role == role).ToList().First();
            
            _context.DeleteObject(entity);
            _context.SaveChanges();
        }

        public void DeleteRole(long userId, int placeId, string role)
        {
            var entity = _context.UserPlaces.Where(ur => ur.UserId == userId && ur.PlaceId == placeId && ur.Role == role).ToList().First();

            _context.DeleteObject(entity);
            _context.SaveChanges();
        }
    }
}
