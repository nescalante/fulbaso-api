using Facebook;

namespace Fulbaso.Facebook.BusinessLogic
{
    public abstract class FacebookService
    {
        protected string Token;
        private FacebookClient _client;

        protected FacebookClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new FacebookClient(Token);
                }

                return _client;
            }
        }

        public void SetToken(string token)
        {
            Token = token;
        }
    }
}
