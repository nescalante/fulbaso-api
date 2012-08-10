using System;
using System.Web;
using System.Web.Security;
using Facebook;

namespace Fulbaso.Common
{
    public class FacebookLogin
    {
        static string usersession = "facebookuser_key";

        public static void Login(string token)
        {
            try
            {
                if (FacebookLogin.User != null && FacebookLogin.User.Token == token) return;

                var fc = new FacebookClient(token);
                var user = fc.Get("me") as dynamic;

                FacebookLogin.User = new FacebookUser
                {
                    Id = Convert.ToInt64(user.id),
                    Name = user.name,
                    UserName = user.username,
                    FirstName = user.first_name,
                    LastName = user.last_name,
                    Token = token,
                };

                if (FacebookLogin.User != null)
                {
                    HttpContext.Current.Session.Remove("Places");
                }
            }
            catch
            {
                FacebookLogin.User = null;
                HttpContext.Current.Session.Remove("Places");
                FormsAuthentication.SignOut();

                throw new UnauthorizedAccessException("Invalid token");
            }
        }

        public static void Logout()
        {
            HttpContext.Current.Session.Remove("Places");
            FormsAuthentication.SignOut();
        }

        public static long Id
        {
            get { return FacebookLogin.User != null ? Convert.ToInt64(FacebookLogin.User.Id) : 0; }
        }

        public static FacebookUser User
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Session[usersession] as FacebookUser;
            }

            private set
            {
                FacebookUtil.SetSessionValue(usersession, value);
            }
        }

        public static string Token
        {
            get
            {
                return User.Token;
            }
        }
    }
}