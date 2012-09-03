using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Fulbaso.Helpers
{
    public class Geocoding
    {
        private const string mapsUrl = "http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false&language=ES";

        public GeocodeResponse Get(string address)
        {
            var url = string.Format(mapsUrl, HttpUtility.UrlEncode(address));

            // make request to the API
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json; charset=utf-8";
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";

            string json;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }

            // read json object
            JObject o = JObject.Parse(json);

            var topLevelStatus = (string)o["status"];

            // check status and handle errors
            if (topLevelStatus == "OK")
            {
                var gr = new GeocodeResponse();
                var results = ((JArray)o["results"]).First();

                foreach (var i in results["address_components"])
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

                var location = (JObject)((JObject)results["geometry"])["location"];

                gr.Latitude = (decimal)location["lat"];
                gr.Longitude = (decimal)location["lng"];
                gr.FormattedAddress = (string)results["formatted_address"];

                return gr;
            }

            throw new Exception("Address not found.");
        }
    }   
}
