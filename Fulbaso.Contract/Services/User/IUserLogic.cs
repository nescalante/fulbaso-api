namespace Fulbaso.Contract
{
    public interface IUserLogic
    {
        void Add(User user);

        void Delete(long userId);

        User Get(long userId);

        void Update(User user);
    }
}
