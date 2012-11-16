using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Place : EntityDataObject
    {
        public string Info { get; set; }

        public string Address { get; set; }

        public Location Location { get; set; }

        public string MapLocation { get; set; }

        public decimal? MapUa { get; set; }

        public decimal? MapVa { get; set; }

        public string Phone { get; set; }

        public string HowToArrive { get; set; }

        public DateTime DateFrom { get; set; }

        public string Page { get; set; }

        public IEnumerable<Service> Services { get; set; }

        public int Courts { get; set; }

        public IEnumerable<Court> CourtsInfo { get; set; }

        public IEnumerable<File> Images { get; set; }

        public string FullAddress
        {
            get
            {
                return this.MapLocation == null ? this.Address : (this.Address + ((this.Location != null && this.Location.ToString() != string.Empty) ? (", " + this.Location + (this.Location.Region.ToString() == this.Location.ToString() ? "" : (", " + this.Location.Region))) : string.Empty));
            }
        }

        public string Url { get; set; }
    }
}
