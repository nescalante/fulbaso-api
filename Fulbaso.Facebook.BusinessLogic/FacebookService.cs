using System;
using Facebook;
using System.Globalization;

namespace Fulbaso.Facebook.Logic
{
    public abstract class FacebookService
    {
        protected string Token;
        private FacebookClient _client;

        public CultureInfo FacebookCulture
        {
            get
            {
                return CultureInfo.GetCultureInfo("en-US");
            }
        }

        protected FacebookClient Client
        {
            get
            {
                if (_client == null)
                {
                    if (Token == null)
                    {
                        _client = new FacebookClient();
                    }
                    else
                    {
                        _client = new FacebookClient(Token);
                    }
                }

                return _client;
            }
        }

        public void SetToken(string token)
        {
            if (token == null) throw new ArgumentNullException("token");

            Token = token;
            _client = null;
        }
    }
}
