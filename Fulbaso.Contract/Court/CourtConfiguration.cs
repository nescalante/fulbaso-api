using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Common;

namespace Fulbaso.Contract
{
    [Serializable]
    public class CourtConfiguration : EntityWithId
    {
        public Court Court { get; set; }

        [DisplayName("Desde la hora:")]
        public byte? StartHour { get; set; }
        
        [DisplayName("Hasta la hora:")]
        public byte? EndHour { get; set; }

        public IEnumerable<DayOfWeek> Days { get; set; }

        [DataType(DataType.Currency)]
        [DisplayName("Precio:")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        public byte Order { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Desde el día:")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Hasta el día:")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
    }
}
