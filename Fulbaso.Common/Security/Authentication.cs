using System;
using System.Web;
using System.Web.Security;
using Fulbaso.Contract;

namespace Fulbaso.Common
{
    public class Authentication
    {
        private IUserService _userService;
        private const string usersession = "user_key";

        public Authentication(IUserService userService)
        {
            _userService = userService;
        }

        public void Login(string token)
        {
            try
            {
                if (Authentication.User != null && Authentication.User.Token == token) return;

                _userService.SetToken(token);
                HttpContext.Current.Session.Remove("Places");
                Authentication.User = _userService.GetUser();
            }
            catch
            {
                Authentication.User = null;
                HttpContext.Current.Session.Remove("Places");
                FormsAuthentication.SignOut();

                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        public void Logout()
        {
            HttpContext.Current.Session.Remove("Places");
            FormsAuthentication.SignOut();
        }

        public static long Id
        {
            get 
            {
                if (Authentication.User == null) throw new InvalidOperationException("No user logged in.");

                return Authentication.User.Id; 
            }
        }

        public static User User
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Session[usersession] as User;
            }

            private set
            {
                HttpContext.Current.Session[usersession] = value;
            }
        }

        public static string Token
        {
            get
            {
                if (Authentication.User == null) throw new InvalidOperationException("No user logged in.");

                return Authentication.User.Token;
            }
        }
    }
}