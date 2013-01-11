using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class CourtGroupModel
    {
        [DataMember(Name = "type", Order = 1)]
        public string CourtType { get; set; }

        [DataMember(Name = "count", Order = 2)]
        public int Count { get; set; }

        [DataMember(Name = "summary", Order = 3)]
        public List<string> Summary { get; set; }

        [DataMember(Name = "detail", Order = 4, EmitDefaultValue = false)]
        public List<CourtSummaryModel> Detail { get; set; }
    }
}