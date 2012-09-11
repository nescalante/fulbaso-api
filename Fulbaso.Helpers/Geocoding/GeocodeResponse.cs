using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Fulbaso.Helpers
{
    public class GeocodeResponse
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string StreetNumber { get; set; }

        public string Route { get; set; }
        
        public string Country { get; set; }

        public string Locality { get; set; }

        public string AdministrativeAreaLevel1 { get; set; }

        public string AdministrativeAreaLevel2 { get; set; }

        public string Neighborhood { get; set; }

        public string FormattedAddress { get; set; }

        public static GeocodeResponse Get(string json)
        {
            try
            {
                return GeocodeResponse.Get(JArray.Parse(json));
            }
            catch
            {
                throw new InvalidOperationException("Could not parse json for that location");
            }
        }

        internal static GeocodeResponse Get(JArray components)
        {
            var gr = new GeocodeResponse();

            foreach (var i in components)
            {
                var type = (string)((JArray)i["types"]).First();

                switch (type)
                {
                    case "street_number":
                        gr.StreetNumber = (string)i["long_name"];
                        break;
                    case "route":
                        gr.Route = (string)i["long_name"];
                        break;
                    case "neighborhood":
                        gr.Neighborhood = (string)i["long_name"];
                        break;
                    case "locality":
                        gr.Locality = (string)i["long_name"];
                        break;
                    case "administrative_area_level_1":
                        gr.AdministrativeAreaLevel1 = (string)i["long_name"];
                        break;
                    case "administrative_area_level_2":
                        gr.AdministrativeAreaLevel2 = (string)i["long_name"];
                        break;
                    case "country":
                        gr.Country = (string)i["long_name"];
                        break;
                }
            }

            return gr;
        }
    }
}
