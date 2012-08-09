using System;
using System.Configuration;

namespace Fulbaso.UI
{
    public class UIConfiguration
    {
        public readonly static int ROWS_COUNT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RowsCount"));

        public readonly static string APP_TITLE = ConfigurationManager.AppSettings.Get("AppTitle");

        public readonly static string APP_ID = ConfigurationManager.AppSettings.Get("AppID");
    }
}
