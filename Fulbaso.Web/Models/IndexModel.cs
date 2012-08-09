using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fulbaso.Contract;

namespace Fulbaso.UI.Models
{
    public class IndexModel
    {
        public IEnumerable<Place> TopUsedPlaces { get; set; }

        public IEnumerable<Place> TopVotedPlaces { get; set; }

        public int PlacesCount { get; set; }

        public int CourtsCount { get; set; }

        public int OwnedPlaces { get; set; }
    }
}