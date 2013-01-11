using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class CourtSummaryModel
    {
        [DataMember(Name = "count", Order = 1)]
        public int Count { get; set; }

        [DataMember(Name = "summary", Order = 2)]
        public List<string> Summary { get; set; }
    }
}
