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

        [Display(Name = "Cantidad de jugadores:")]
        public int? Players { get; set; }

        [Display(Name = "Deporte:")]
        public EntityDataObject CourtType { get; set; }

        [Display(Name = "Tipo de suelo:")]
        public EntityDataObject FloorType { get; set; }

        [Display(Name = "Techada:")]
        public bool IsIndoor { get; set; }

        [Display(Name = "Iluminada:")]
        public bool IsLighted { get; set; }

        [Display(Name = "Activa:")]
        public override bool IsActive { get; set; }

        public IEnumerable<CourtConfiguration> Configuration { get; set; }

        public IEnumerable<CourtBook> Books { get; set; }
    }
}
