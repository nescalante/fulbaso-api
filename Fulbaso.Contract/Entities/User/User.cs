using System;
using System.Collections.Generic;

namespace Fulbaso.Contract
{
    [Serializable]
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<Tuple<int, string>> PlaceRoles { get; set; }

        public DateTime? Birthday { get; set; }

        public string Token { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastLogin { get; set; }
    }
}