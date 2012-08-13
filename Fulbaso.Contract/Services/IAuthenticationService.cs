namespace Fulbaso.Contract
{
    public interface IAuthenticationService
    {
        User GetUser();

        void SetToken(string token);
    }
}
