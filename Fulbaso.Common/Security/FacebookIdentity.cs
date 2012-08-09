using System.Security.Principal;

namespace Fulbaso.Common
{
    public class FacebookIdentity : IIdentity
    {
        public FacebookIdentity(string name, long id, string token)
        {
            this.Name = name;
            this.Id = id;
            this.Token = token;
        }

        public string AuthenticationType
        {
            get { return "Facebook"; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(this.Name); }
        }

        public string Name
        {
            get;
            private set;
        }

        public long Id
        {
            get;
            private set;
        }

        public string Token
        {
            get;
            private set;
        }
    }
}