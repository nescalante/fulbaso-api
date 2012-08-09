using System.Collections.Generic;

namespace Fulbaso.Helpers
{
    public class CaseInsensitiveComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.ToLower() == x.ToLower();
        }

        public int GetHashCode(string obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}
