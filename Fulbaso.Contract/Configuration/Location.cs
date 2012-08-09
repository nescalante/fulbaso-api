using System;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Common;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Location : EntityDataObject
    {
        [Display(Name = "Ciudad:")]
        public Region Region { get; set; }
    }
}
