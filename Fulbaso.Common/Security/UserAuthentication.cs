using System;
using System.Web;
using System.Web.Security;
using Fulbaso.Contract;

namespace Fulbaso.Common
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
                if (UserAuthentication.User != null && UserAuthentication.User.Token == token) return;

                _userService.SetToken(token);
                HttpContext.Current.Session.Remove("Places");
                UserAuthentication.User = _userService.GetUser();
            }
            catch
            {
                UserAuthentication.User = null;
                HttpContext.Current.Session.Remove("Places");
                FormsAuthentication.SignOut();

                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        public void Logout()
        {
            UserAuthentication.User = null;
            HttpContext.Current.Session.Remove("Places");
            FormsAuthentication.SignOut();
        }

        public static long UserId
        {
            get 
            {
                if (UserAuthentication.User == null) throw new InvalidOperationException("No user logged in.");

                return UserAuthentication.User.Id; 
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
                if (UserAuthentication.User == null) throw new InvalidOperationException("Cant get user token, no user logged in.");

                return UserAuthentication.User.Token;
            }
        }
    }
}