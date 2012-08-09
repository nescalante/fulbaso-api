using System;
using Fulbaso.Common;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Client : EntityDataObject
    {
        public string Phone { get; set; }

        public Place Place { get; set; }

        public int Books { get; set; }

        public DateTime? LastBook { get; set; }
    }
}
