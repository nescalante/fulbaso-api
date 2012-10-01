using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class CourtConfiguration : EntityWithId
    {
        public Court Court { get; set; }

        public byte? StartHour { get; set; }
        
        public byte? EndHour { get; set; }

        public IEnumerable<DayOfWeek> Days { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        public byte Order { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
    }
}
