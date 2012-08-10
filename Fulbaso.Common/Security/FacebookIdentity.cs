using System.Security.Principal;

namespace Fulbaso.Common
{
    public class FacebookIdentity : IIdentity
    {
        public FacebookIdentity(FacebookUser user)
        {
            this.Name = user.Name;
            this.Id = user.Id;
            this.Token = user.Token;
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