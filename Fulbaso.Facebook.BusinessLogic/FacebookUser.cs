using System;
using System.Globalization;
using Fulbaso.Contract;

namespace Fulbaso.Facebook.Logic
{
    public class FacebookUser : FacebookLogic
    {
        public User GetUser()
        {
            if (this.Client == null) throw new InvalidOperationException("Client not initialized.");

            DateTime bd = DateTime.Now;
            var user = this.Client.Get("me") as dynamic;
            var userDto = new User
            {
                Id = Convert.ToInt64(user.id),
                Name = user.name,
                UserName = user.username,
                FirstName = user.first_name,
                LastName = user.last_name,
                Email = user.email,
                Token = this.Token,
            };

            if (user.birthday != null && DateTime.TryParse(user.birthday, FacebookCulture.DateTimeFormat, DateTimeStyles.None, out bd))
                userDto.Birthday = bd;
            else
                userDto.Birthday = null;

            return userDto;
        }
    }
}
