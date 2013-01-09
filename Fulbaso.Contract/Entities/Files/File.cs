using System;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    [Serializable]
    public class File : EntityWithId
    {
        public string FileName { get; set; }

        public string Description { get; set; }

        public int ContentLength { get; set; }

        public string ContentType { get; set; }

        public User CreatedBy { get; set; }

        public DateTime InsertDate { get; set; }
    }
}
