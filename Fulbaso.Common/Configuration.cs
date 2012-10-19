using System;
using System.Configuration;

namespace Fulbaso.Common
{
    public class Configuration
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public readonly static int RowsPerRequest = Convert.ToInt32(Get("RowsPerRequest"));

        public readonly static string AppTitle = Get("AppTitle");

        public readonly static string AppSecret = Get("AppSecret");

        public readonly static string AppId = Get("AppId");

        public static bool IgnoreMinification
        {
            get { return Convert.ToBoolean(Get("IgnoreMinification") ?? "false"); }
        }
    }
}
