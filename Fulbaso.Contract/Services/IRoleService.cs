namespace Fulbaso.Contract
{
    public interface IRoleService
    {
        void AddRole(long userId, int placeId, string role);

        void AddRole(long userId, string role);

        void DeleteRole(long userId, int placeId, string role);

        void DeleteRole(long userId, string role);

        bool IsUserInRole(long userId, int placeId, string role);

        bool IsUserInRole(long userId, string role);
    }
}
