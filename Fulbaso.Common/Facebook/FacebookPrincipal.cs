﻿using System;
using System.Security.Principal;

namespace Fulbaso.Common
{
    public class FacebookPrincipal : IPrincipal
    {
        public FacebookPrincipal(FacebookIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException("identity");

            this.Identity = identity;
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}