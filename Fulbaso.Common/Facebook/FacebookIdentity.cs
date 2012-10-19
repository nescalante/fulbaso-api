using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Fulbaso.Contract;

namespace Fulbaso.Common
{
    public class FacebookIdentity : IIdentity
    {
        public FacebookIdentity(User user)
        {
            this.Name = user.Name;
            this.Id = user.Id;
            this.Token = user.Token;
            this.Roles = user.Roles != null ? user.Roles.ToArray() : new string[] { };
            this.PlaceRoles = user.PlaceRoles != null ? user.PlaceRoles.ToArray() : new List<Tuple<int, string>>().ToArray();
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

        public string[] Roles
        {
            get;
            private set;
        }

        public Tuple<int, string>[] PlaceRoles
        {
            get;
            private set;
        }
    }
}