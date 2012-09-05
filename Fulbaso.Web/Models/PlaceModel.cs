using System;
using System.Collections.Generic;
using Fulbaso.Contract;

namespace Fulbaso.Web.Models
{
    public class PlaceModel
    {
        public Place Place { get; set; }

        public bool HasAdmin { get; set; }

        public bool IsFavourite { get; set; }
    }
}