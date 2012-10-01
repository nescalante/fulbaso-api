using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Court : EntityDataObject
    {
        public Place Place { get; set; }

        public int? Players { get; set; }

        public CourtType CourtType { get; set; }

        public FloorType FloorType { get; set; }

        public bool IsIndoor { get; set; }

        public bool IsLighted { get; set; }

        public override bool IsActive { get; set; }

        public IEnumerable<CourtConfiguration> Configuration { get; set; }

        public IEnumerable<CourtBook> Books { get; set; }
    }
}
