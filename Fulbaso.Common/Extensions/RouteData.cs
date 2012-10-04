using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Fulbaso.Common
{
    public static class RouteDataExtensions
    {
        public static string GetControllerString(this RouteData data, string defaultText = "")
        {
            string controller = data.GetRequiredString("controller", defaultText);
            return controller;
        }

        public static string GetActionString(this RouteData data, string defaultText = "")
        {
            string action = data.GetRequiredString("action", defaultText);
            return action;
        }

        private static string GetRequiredString(this RouteData data, string key, string defaultText)
        {
            string required = data.GetRequiredString(key);
            if (string.IsNullOrWhiteSpace(required))
            {
                required = defaultText;
            }
            return required;
        }
    }
}
