using Fulbaso.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Fulbaso.Service.Contract
{
    [DataContract]
    public class PlaceModel
    {
        [DataMember(Name = "name", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "description", Order = 2, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Name = "address", Order = 3, EmitDefaultValue = false)]
        public string Address { get; set; }

        [DataMember(Name = "phone", Order = 4, EmitDefaultValue = false)]
        public string Phone { get; set; }

        [DataMember(Name = "how_to_arrive", Order = 5, EmitDefaultValue = false)]
        public string HowToArrive { get; set; }

        [DataMember(Name = "page", Order = 6)]
        public string Page { get; set; }

        [DataMember(Name = "lat", Order = 7, EmitDefaultValue = false)]
        public decimal? Latitude { get; set; }

        [DataMember(Name = "lng", Order = 8, EmitDefaultValue = false)]
        public decimal? Longitude { get; set; }

        [DataMember(Name = "services", Order = 9)]
        public List<string> Services { get; set; }

        [DataMember(Name = "courts", Order = 10)]
        public List<CourtGroupModel> Courts { get; set; }

        private static string GetJoin(IEnumerable<string> list)
        {
            return list.Count() > 1 ? string.Join(", ", list.Take(list.Count() - 1)) + " y " + list.Last() : list.First();
        }

        private static List<string> GetSummary(IGrouping<string, Court> group)
        {
            var summary = new List<string>();
            var list = group.ToList();
            var info = list.GroupBy(g => new { 
                FloorType = g.FloorType.Description, 
                IsLighted = g.IsLighted, 
                IsIndoor = g.IsIndoor, 
                g.Players 
            });

            if (info.All(i => i.Key.Players != null))
            {
                summary.Add(GetJoin(info.Select(i => i.Key.Players).OrderBy(i => i).Distinct().Select(i => i.ToString())) + " jugadores");
            }
            
            if (list.GroupBy(i => i.FloorType.Description).Count() == 1)
            {
                summary.Add(list.First().FloorType.ToString());
            }

            if (list.GroupBy(i => i.IsIndoor).Count() == 1 && list.First().IsIndoor)
            {
                summary.Add("Techada" + (list.Count() > 1 ? "s" : ""));
            }

            if (list.GroupBy(i => i.IsLighted).Count() == 1 && list.First().IsLighted)
            {
                summary.Add("Con luz");
            }

            return summary.Count() > 0 ? summary : null;
        }

        private static List<CourtSummaryModel> GetDetail(IGrouping<string, Court> group)
        {
            var detail = new List<CourtSummaryModel>();
            var info = group.ToList().GroupBy(g => new { 
                FloorType = g.FloorType.Description, 
                IsLighted = g.IsLighted, 
                IsIndoor = g.IsIndoor, 
                g.Players 
            });

            foreach (var i in info.OrderByDescending(i => i.Count()).ThenByDescending(i => i.Key.Players))
            {
                var list = new List<string>();

                if (i.Key.Players != null)
                {
                    list.Add(i.Key.Players + " jugadores");
                }
                list.Add(i.Key.FloorType.ToString());
                if (i.Key.IsIndoor)
                {
                    list.Add("Techada" + (i.Count() > 1 ? "s" : ""));
                }
                if (i.Key.IsLighted)
                {
                    list.Add("Con luz");
                }

                detail.Add(new CourtSummaryModel {
                    Count = i.Count(),
                    Summary = list,
                });
            }

            return detail;
        }

        public static explicit operator PlaceModel(Place place)
        {
            var model = new PlaceModel
            {
                Name = place.Description,
                Description = place.Info,
                Address = place.Address,
                Phone = place.Phone,
                HowToArrive = place.HowToArrive,
                Page = place.Page,
                Latitude = place.MapUa,
                Longitude = place.MapVa,
                Services = place.Services.Select(i => i.ToString()).ToList(),
            };

            model.Courts = place.CourtsInfo
                .Where(i => i.IsActive)
                .OrderBy(i => i.CourtType.Id)
                .GroupBy(i => i.CourtType.Description)
                .Select(i => new CourtGroupModel {
                    CourtType = i.Key,
                    Count = i.Count(),
                    Summary = GetSummary(i),
                    Detail = GetDetail(i),
                }).ToList();

            return model;
        }
    }
}