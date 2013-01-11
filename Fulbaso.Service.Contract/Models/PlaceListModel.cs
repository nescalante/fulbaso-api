using Fulbaso.Contract;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class PlaceListModel
    {
        [DataMember(Name = "title", Order = 1, EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(Name = "description", Order = 2, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Name = "count", Order = 3)]
        public int Count { get; set; }

        [DataMember(Name = "data", Order = 4)]
        public List<PlaceListItemModel> List { get; set; }
    }
}