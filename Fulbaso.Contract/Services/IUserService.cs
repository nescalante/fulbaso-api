namespace Fulbaso.Contract
{
    public interface IUserService
    {
        void Add(User user);

        void Delete(long userId);

        User Get(long userId);

        void Update(User user);
    }
}
