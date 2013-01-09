using System;
using System.Net;
using System.Web;
using System.Web.Security;
using Fulbaso.Contract;

namespace Fulbaso.Common.Security
{
    public class UserAuthentication
    {
        private IAuthenticationLogic _authentication;
        private IUserLogic _userService;
        private const string usersession = "user_key";

        public UserAuthentication(IAuthenticationLogic authentication, IUserLogic userService)
        {
            _authentication = authentication;
            _userService = userService;
        }

        public void Login(string token)
        {
            try
            {
                if (this.User != null && this.User.Token == token) return;

                _authentication.SetToken(token);
                HttpContext.Current.Session.Remove("Places");
                this.User = _authentication.GetUser();

                HttpContext.Current.Response.Cookies["user"]["id"] = this.User.Id.ToString();
                HttpContext.Current.Response.Cookies["user"]["token"] = this.User.Token;
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
            HttpContext.Current.Response.Cookies["user"].Expires = DateTime.Now.AddDays(-1D);
            HttpContext.Current.Session.Remove("Places");
            FormsAuthentication.SignOut();
        }

        private User User
        {
            get
            {
                if (HttpContext.Current == null) return null;

                var user = HttpContext.Current.Session[usersession] as User;

                if (user != null)
                {
                    return user;
                }
                else
                {
                    var cookie = HttpContext.Current.Request.Cookies["user"];
                    long userId;

                    if (cookie != null && cookie["id"] != null && long.TryParse(cookie["id"], out userId))
                    {
                        _authentication.SetToken(cookie["id"]);
                        user = _userService.Get(userId);
                        HttpContext.Current.Session[usersession] = user;

                        return user;
                    }
                }

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

        public void Refresh()
        {
            HttpContext.Current.Session[usersession] = _userService.Get(this.User.Id);
            HttpContext.Current.Session["Places"] = null;
        }
    }
}