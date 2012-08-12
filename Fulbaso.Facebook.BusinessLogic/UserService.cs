using System;
using Fulbaso.Contract;

namespace Fulbaso.Facebook.BusinessLogic
{
    public class UserService : FacebookService, IUserService
    {
        public User GetUser()
        {
            var user = this.Client.Get("me") as dynamic;

            return new User
            {
                Id = Convert.ToInt64(user.id),
                Name = user.name,
                UserName = user.username,
                FirstName = user.first_name,
                LastName = user.last_name,
                Token = this.Token,
            };
        }
    }
}
