namespace Fulbaso.Contract
{
    public interface IAuthenticationLogic
    {
        User GetUser();

        void SetToken(string token);
    }
}
