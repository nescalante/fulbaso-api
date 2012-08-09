using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Helpers;

namespace Fulbaso.UI
{
    public static class InterfaceUtil
    {
        public static bool CheckRoute(this ViewContext context, string action, string controller)
        {
            return (action == null || context.Controller.ValueProvider.GetValue("action").RawValue.ToString().ToUpper() == action.ToUpper()) &&
                (controller == null || context.Controller.ValueProvider.GetValue("controller").RawValue.ToString().ToUpper() == controller.ToUpper());
        }

        public static IEnumerable<T> GetOrdered<T>(this IEnumerable<T> list) where T : IEntity
        {
            return list.OrderBy(i => i.Description).Where(i => i.IsActive).ToList();
        }

        public static string GetJoin(this IEnumerable<string> list)
        {
            return list.Count() > 1 ? string.Join(", ", list.Take(list.Count() - 1)) + " y " + list.Last() : list.First();
        }

        public static string GetRemarked(string description, string remark, string className)
        {
            if (description == null) return string.Empty;

            var result = description;
            var tagInit = "<span class=\"" + className + "\">";
            var tagEnd = "</span>";

            if (!string.IsNullOrEmpty(remark))
            {
                var val = remark.Trim().ToLower();
                var ix = description.ToLower().IndexOf(val);

                while (ix >= 0)
                {
                    result = result.Substring(0, ix) + tagInit + result.Substring(ix, remark.Trim().Length) + tagEnd +
                        result.Substring(ix + val.Length);
                    if (result.Substring(ix + (val + tagInit + tagEnd).Length).ToLower().IndexOf(val) < 0)
                        ix = -1;
                    else
                        ix = result.Substring(ix + (val + tagInit + tagEnd).Length).ToLower().IndexOf(val) + (val + tagInit + tagEnd).Length + ix;
                }
            }

            return result;
        }

        public static string GetDescription(this Enum value)
        {
            return EnumHelper.GetDescription(value);
        }

        internal static int[] GetInts(FormCollection collection, string tag)
        {
            return collection.AllKeys.Where(k => k.StartsWith(tag) && Convert.ToBoolean(collection[k].Split(',').First()))
                .Select(k => Convert.ToInt32(k.Split('-').Last())).ToArray();
        }

        internal static int[] GetInts(string list)
        {
            int x;
            return (list ?? "").Split(';').Where(i => !string.IsNullOrEmpty(i) && int.TryParse(i, out x)).Select(i => Convert.ToInt32(i)).ToArray();
        }
    }
}
