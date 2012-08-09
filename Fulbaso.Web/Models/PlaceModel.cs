using System;
using System.Collections.Generic;
using Fulbaso.Contract;

namespace Fulbaso.UI.Models
{
    public class PlaceModel
    {
        public Place Place { get; set; }

        public bool HasAdmin { get; set; }

        public bool IsFavourite { get; set; }

        public IEnumerable<Tuple<Place, double?>> NearPlaces { get; set; }
    }
}