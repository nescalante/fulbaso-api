using System;
using System.ComponentModel.DataAnnotations;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class Location : EntityDataObject
    {
        public Region Region { get; set; }
    }
}
