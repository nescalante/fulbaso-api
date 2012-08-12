using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Place : EntityDataObject
    {
        [Display(Name = "Información:")]
        public string Info { get; set; }

        [Display(Name = "Dirección:")]
        public string Address { get; set; }

        [Display(Name = "Localidad:")]
        public Location Location { get; set; }

        [Display(Name = "Localización:")]
        public string MapLocation { get; set; }

        [Display(Name = "Latitud:")]
        public decimal? MapUa { get; set; }

        [Display(Name = "Longitud:")]
        public decimal? MapVa { get; set; }

        [Display(Name = "Teléfono:")]
        public string Phone { get; set; }

        [Display(Name = "¿Cómo llegar?:")]
        public string HowToArrive { get; set; }

        [Display(Name = "Nombre de la página:")]
        public string Page { get; set; }

        public IEnumerable<Service> Services { get; set; }

        [Display(Name = "Cantidad de canchas:")]
        public int Courts { get; set; }

        public IEnumerable<Court> CourtsInfo { get; set; }
    }
}
