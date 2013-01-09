using System;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class CourtBook : EntityWithId
    {
        public Court Court { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal Price { get; set; }

        public bool ReserveRequired { get; set; }

        public decimal? Reserve { get; set; }

        public decimal? Paid { get; set; }

        public Client Client { get; set; }

        public string Comment { get; set; }

        public long User { get; set; }
    }
}
