using System;
using System.Web;
using System.Web.SessionState;

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

        public static void Set(decimal lat, decimal lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
    }
}
