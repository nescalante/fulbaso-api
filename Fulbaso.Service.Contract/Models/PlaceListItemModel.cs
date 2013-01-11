using Fulbaso.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class PlaceListItemModel
    {
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "address", Order = 2, EmitDefaultValue = false)]
        public string Address { get; set; }

        [DataMember(Name = "phone", Order = 3, EmitDefaultValue = false)]
        public string Phone { get; set; }

        [DataMember(Name = "page", Order = 4)]
        public string Page { get; set; }

        [DataMember(Name = "lat", Order = 5, EmitDefaultValue = false)]
        public decimal? Latitude { get; set; }

        [DataMember(Name = "lng", Order = 6, EmitDefaultValue = false)]
        public decimal? Longitude { get; set; }

        [DataMember(Name = "courts_count", Order = 7)]
        public int Courts { get; set; }

        [DataMember(Name = "services", Order = 8)]
        public List<string> Services { get; set; }

        public static explicit operator PlaceListItemModel(Place place)
        {
            return new PlaceListItemModel
            {
                Name = place.Description,
                Address = place.Address,
                Phone = place.Phone,
                Page = place.Page,
                Latitude = place.MapUa,
                Longitude = place.MapVa,
                Courts = place.Courts,
                Services = place.Services.Select(i => i.ToString()).ToList(),
            };
        }
    }
}