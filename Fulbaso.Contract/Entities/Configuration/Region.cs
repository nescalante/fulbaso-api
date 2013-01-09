using System;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Region : EntityDataObject
    {
        public EntityDataObject Territory { get; set; }
    }
}
