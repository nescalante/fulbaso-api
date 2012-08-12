using System;

namespace Fulbaso.Contract
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }

        public DateTime Created { get; set; }
    }
}