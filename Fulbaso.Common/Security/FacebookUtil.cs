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

        public static string Serialize<T>(T obj, IEnumerable<Type> knownTypes = null)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = knownTypes == null
                                ? new DataContractJsonSerializer(typeof(T))
                                : new DataContractJsonSerializer(typeof(T), knownTypes);
                serializer.WriteObject(ms, obj);

                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static T Deserialize<T>(HttpWebResponse response, IEnumerable<Type> knownTypes = null)
        {
            if (response.ContentLength == 0)
            {
                return default(T);
            }

            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                {
                    return default(T);
                }

                using (var reader = new StreamReader(responseStream))
                {
                    var json = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(json))
                    {
                        return default(T);
                    }
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                    {
                        var serializer = knownTypes == null
                                        ? new DataContractJsonSerializer(typeof(T))
                                        : new DataContractJsonSerializer(typeof(T), knownTypes);
                        return (T)serializer.ReadObject(ms);
                    }
                }
            }
        }

        public static T CreateCall<T>(string url, string httpMethod = "GET")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Proxy = new WebProxy("192.168.0.1", 8080);
            request.Method = httpMethod.ToUpper();

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                return Deserialize<T>(response);
            }
        }
    }
}
