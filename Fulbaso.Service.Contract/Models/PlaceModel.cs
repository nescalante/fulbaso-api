using Fulbaso.Contract;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PlaceService = Fulbaso.Contract.Service;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class PlaceModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "courts_count")]
        public int Courts { get; set; }

        //[DataMember]
        //public List<PlaceService> Services { get; set; }

        public static explicit operator PlaceModel(Place place)
        {
            return new PlaceModel
            {
                Name = place.Description,
                Address = place.Address,
                Phone = place.Phone,
                Courts = place.Courts,
            };
        }
    }
}