using Fulbaso.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceTag = Fulbaso.Contract.Service;

namespace Fulbaso.Service
{
    public static class FilterUtil
    {
        public static string GetJoin(this IEnumerable<string> list)
        {
            return list.Count() > 1 ? string.Join(", ", list.Take(list.Count() - 1)) + " y " + list.Last() : list.First();
        }

        public static string GetDescription(this Enum value)
        {
            return EnumHelper.GetDescription(value);
        }

        internal static int[] GetInts(this string list, char separator)
        {
            int x;
            return (list ?? "").Split(separator).Where(i => !string.IsNullOrEmpty(i) && int.TryParse(i, out x)).Select(i => Convert.ToInt32(i)).ToArray();
        }

        internal static decimal? GetDecimal(this string d)
        {
            decimal x;
            return decimal.TryParse(d ?? "", out x) ? (decimal?)x : null;
        }

        internal static byte[] GetBytes(this string list, char separator)
        {
            int x;
            return (list ?? "").Split(separator).Where(i => !string.IsNullOrEmpty(i) && int.TryParse(i, out x)).Select(i => Convert.ToByte(i)).ToArray();
        }

        public static string GetFloors(this IEnumerable<int> list)
        {
            throw new Exception();
            //return ContainerUtil.GetApplicationContainer().Resolve<IFloorTypeLogic>().Get().Where(f => list.Contains(f.Id)).Select(f => f.ToString()).GetJoin();
        }

        public static string GetTags(this IEnumerable<byte> list)
        {
            return list.Select(i => ((ServiceTag)i).GetDescription()).GetJoin();
        }
    }
}


