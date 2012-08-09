using System;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Common;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Region : EntityDataObject
    {
        [Display(Name = "País:")]
        public EntityDataObject Territory { get; set; }
    }
}
