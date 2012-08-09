using System;
using System.Web;
using System.Web.Security;

namespace Fulbaso.Common
{
    public class FacebookLogin
    {
        static string usersession = "facebookuser_key";
        static string tokensession = "token_key";

        public static void Login(string token)
        {
            if (Token != token)
            {
                Token = token;
                HttpContext.Current.Cache.Remove("Places");
                FacebookLogin.User = null;
            }

            if (FacebookLogin.User != null)
            {
                HttpContext.Current.Cache.Remove("Places");
            }
        }

        public static void Logout()
        {
            HttpContext.Current.Cache.Remove("Places");
            FormsAuthentication.SignOut();
            Token = null;
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

                var user = HttpContext.Current.Session[usersession];

                if (user != null) return user as FacebookUser;

                if (!string.IsNullOrEmpty(Token))
                {
                    var obj = new FacebookUser { Id = "15", UserName = "test", Name = "test", }; // FacebookUtil.CreateCall<FacebookUser>("https://graph.facebook.com/me/?access_token=" + Token);

                    HttpContext.Current.Session[usersession] = obj;

                    return obj;
                }

                return null;
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
                return FacebookUtil.GetSessionValue<string>(tokensession);
            }
            set
            {
                FacebookUtil.SetSessionValue(tokensession, value);

                if (value == null)
                {
                    FacebookLogin.User = null;
                }
            }
        }
    }
}