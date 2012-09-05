using System;
using System.Configuration;

namespace Fulbaso.Web
{
    public class Configuration
    {
        public readonly static int RowsPerRequest = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RowsPerRequest"));

        public readonly static string AppTitle = ConfigurationManager.AppSettings.Get("AppTitle");

        public readonly static string AppId = ConfigurationManager.AppSettings.Get("AppId");
    }
}
