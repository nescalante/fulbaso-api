using System;
using System.Web;
using System.Web.SessionState;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.Common
{
    public class Position
    {
        private static HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        public static bool HasValue
        {
            get
            {
                return Session["Latitude"] != null && Session["Longitude"] != null;
            }
        }

        public static decimal Latitude
        {
            get
            {
                if (Session["Latitude"] == null)
                {
                    throw new ArgumentNullException("Latitude not set");
                }

                return (decimal)Session["Latitude"];
            }
            private set
            {
                Session["Latitude"] = value;
            }
        }

        public static decimal Longitude
        {
            get
            {
                if (Session["Longitude"] == null)
                {
                    throw new ArgumentNullException("Longitude not set");
                }

                return (decimal)Session["Longitude"];
            }
            private set
            {
                Session["Longitude"] = value;
            }
        }

        public static Location Location
        {
            get
            {
                if (!Position.HasValue) return null;

                var location = Session["Location"] as Location;

                if (location != null)
                {
                    if (location.Id == 0) return null;
                    else return location;
                }

                try
                {
                    var gr = new Geocoding().Get(Position.Latitude, Position.Longitude);
                    Session["Location"] = gr.GetLocation();
                }
                catch
                {
                    Session["Location"] = EntityDataObject.Create<Location>(0);
                }

                return Session["Location"] as Location;
            }
        }

        public static void Set(decimal lat, decimal lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
    }
}
