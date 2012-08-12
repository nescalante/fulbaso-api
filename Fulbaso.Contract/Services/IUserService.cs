namespace Fulbaso.Contract
{
    public interface IUserService
    {
        User GetUser();

        void SetToken(string token);
    }
}
