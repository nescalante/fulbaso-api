﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fulbaso.Contract;
using System.Linq.Expressions;

namespace Fulbaso.EntityFramework.Logic
{
    public class UserService : IUserService
    {
        public void Add(User user)
        {
            var userEntity = new UserEntity
            {
                Id = user.Id,
                Name = user.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Birthday = user.Birthday,
                Token = user.Token,
                Created = DateTime.Now,
                LastLogin = DateTime.Now,
            };

            Repository<UserEntity>.Add(userEntity);
            user.Id = userEntity.Id;
            user.Created = userEntity.Created;
            user.LastLogin = userEntity.LastLogin;
        }

        public void Update(User user)
        {
            var userEntity = EntityUtil.Context.Users.Where(u => u.Id == user.Id).ToList().First();

            userEntity.Name = user.Name;
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.UserName = user.UserName;
            userEntity.Email = user.Email;
            userEntity.Birthday = user.Birthday;
            userEntity.Token = user.Token;
            userEntity.Created = user.Created;
            userEntity.LastLogin = user.LastLogin;

            EntityUtil.Context.SaveChanges();
        }

        public void Delete(long userId)
        {
            UserService.Delete(new User { Id = userId, });
        }

        private static void Delete(User user)
        {
            Repository<UserEntity>.Delete(new UserEntity { Id = user.Id });
        }

        public User Get(long userId)
        {
            return UserService.Get(u => u.Id == userId).SingleOrDefault();
        }

        internal static IEnumerable<User> Get(Expression<Func<UserEntity, bool>> predicate)
        {
            var query = Repository<UserEntity>.GetQuery(predicate);
            return UserService.Get(query);
        }

        internal static IEnumerable<User> Get(IQueryable<UserEntity> query)
        {
            return (from u in query.ToList()
                    select new User
                    {
                        Id = u.Id,
                        Name = u.Name,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        UserName = u.UserName,
                        Email = u.Email,
                        Birthday = u.Birthday,
                        Created = u.Created,
                        LastLogin = u.LastLogin,
                        Token = u.Token,
                    }).ToList();
        }
    }
}