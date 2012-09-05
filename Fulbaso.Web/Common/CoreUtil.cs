using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;
using System.Web.Mvc;

namespace Fulbaso.Web
{
    public static class CoreUtil
    {
        public static string GetFloors(this IEnumerable<int> list)
        {
            return ContainerUtil.GetApplicationContainer().Resolve<IFloorTypeService>().Get().Where(f => list.Contains(f.Id)).Select(f => f.ToString()).GetJoin();
        }

        public static string GetTags(this IEnumerable<byte> list)
        {
            return list.Select(i => ((Service)i).GetDescription()).GetJoin();
        }

        public static List<Place> GetPlaces(this IPrincipal user)
        {
            if (user != null && !string.IsNullOrEmpty(user.Identity.Name))
            {
                if (HttpContext.Current.Session["Places"] == null)
                {
                    HttpContext.Current.Session["Places"] = ContainerUtil.GetApplicationContainer().Resolve<IPlaceService>().GetByUser(UserAuthentication.UserId);
                }

                return HttpContext.Current.Session["Places"] as List<Place>;
            }
            else
                return null;
        }

        public static IEnumerable<Tuple<Place, double?>> WithUrl(this IEnumerable<Tuple<Place, double?>> list)
        {
            foreach (var i in list) i.Item1.Url = GetPlaceUrl(i.Item1.Page);

            return list;
        }

        public static IEnumerable<Place> WithUrl(this IEnumerable<Place> list)
        {
            foreach (var i in list) i.Url = GetPlaceUrl(i.Page);

            return list;
        }

        private static string GetPlaceUrl(string page)
        {
            var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            return helper.Action("ItemView", "Place", new { place = page });
        }

        public static int[] GetRange(IEnumerable<Court> courts, DateTime time)
        {
            var configs = courts.SelectMany(c => c.Configuration).Where(c => (c.StartDate == null || c.StartDate <= time) && (c.EndDate == null || c.EndDate >= time) && c.Days.Contains(time.DayOfWeek));
            var mins = configs.Where(c => c.StartHour != null).Select(c => (byte)c.StartHour);
            var maxs = configs.Where(c => c.EndHour != null).Select(c => (byte)c.EndHour);

            int min = mins.Count() > 0 ? mins.Min() : 0;
            int max = maxs.Count() > 0 ? maxs.Max() : 24;

            return min <= max ? Enumerable.Range(min, max - min).ToArray() : new int[] { };
        }

        public static decimal GetPrice(IEnumerable<CourtConfiguration> configurations, DateTime time)
        {
            return configurations.Where(c => (c.StartHour == null || c.StartHour <= time.Hour) &&
                (c.EndHour == null || c.EndHour >= time.Hour) &&
                (c.StartDate == null || c.StartDate <= time) &&
                (c.EndDate == null || c.EndDate >= time) &&
                c.Days.Contains(time.DayOfWeek)).OrderBy(c => c.Order).Select(c => c.Price).FirstOrDefault();
        }
    }
}