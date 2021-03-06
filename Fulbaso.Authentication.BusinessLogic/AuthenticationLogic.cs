﻿using System;
using Fulbaso.Contract;
using Fulbaso.Facebook.Logic;

namespace Fulbaso.Authentication.Logic
{
    public class AuthenticationLogic : IAuthenticationLogic
    {
        private FacebookUser _facebookUser;
        private IUserLogic _userService;

        public AuthenticationLogic(FacebookUser facebookUser, IUserLogic userService)
        {
            _facebookUser = facebookUser;
            _userService = userService;
        }

        public User GetUser()
        {
            // get user from graph
            var graphUser = _facebookUser.GetUser();

            // get user from service
            var user = _userService.Get(graphUser.Id);

            if (user == null)
            {
                // create user
                _userService.Add(graphUser);
            }
            else
            {
                // update user data with service
                graphUser.Created = user.Created;
                graphUser.LastLogin = DateTime.Now;
                graphUser.Roles = user.Roles;
                graphUser.PlaceRoles = user.PlaceRoles;
                _userService.Update(graphUser);
            }

            return graphUser;
        }

        public void SetToken(string token)
        {
            _facebookUser.SetToken(token);
        }
    }
}
