using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace Fulbaso.Common
{
    public static class FacebookUtil
    {
        public static T GetSessionValue<T>(string sessionValue) where T : class
        {
            if (HttpContext.Current == null) return null;

            var value = HttpContext.Current.Session[sessionValue];

            if (value != null) return value as T;

            return null;
        }

        public static void SetSessionValue<T>(string sessionValue, T value) where T : class
        {
            HttpContext.Current.Session[sessionValue] = value;
        }
    }
}
