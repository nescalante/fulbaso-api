using System;
using System.Net;
using System.Web;
using System.Web.Security;
using Fulbaso.Contract;

namespace Fulbaso.Common.Security
{
    public class UserAuthentication
    {
        private IAuthenticationService _userService;
        private const string usersession = "user_key";

        public UserAuthentication(IAuthenticationService userService)
        {
            _userService = userService;
        }

        public void Login(string token)
        {
            try
            {
                if (this.User != null && this.User.Token == token) return;

                _userService.SetToken(token);
                HttpContext.Current.Session.Remove("Places");
                this.User = _userService.GetUser();
            }
            catch
            {
                this.User = null;
                HttpContext.Current.Session.Remove("Places");
                FormsAuthentication.SignOut();

                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        public void Logout()
        {
            this.User = null;
            HttpContext.Current.Session.Remove("Places");
            FormsAuthentication.SignOut();
        }

        private User User
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Session[usersession] as User;
            }
            set
            {
                HttpContext.Current.Session[usersession] = value;
            }
        }

        public User GetUser()
        {
            return this.User;
        }
    }
}