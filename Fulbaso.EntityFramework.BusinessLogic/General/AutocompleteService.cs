using System.Collections.Generic;
using System.Linq;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    internal static class AutocompleteService
    {
        internal static IEnumerable<string> GetForAutocomplete(string prefixText, int count)
        {
            var comparer = new CaseInsensitiveComparer();

            var places = EntityUtil.Context.Places.Where(p => p.Name.Contains(prefixText))
                .OrderBy(p => p.Name.ToLower().IndexOf(prefixText.ToLower()))
                .Take(count).Select(p => p.Name).ToList().Distinct(comparer);

            if (places.Count() == count) return places.ToList();

            var remaining = count - places.Count();
            var locations = EntityUtil.Context.Locations.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            var list = places.Concat(locations).OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            if (locations.Count() == remaining) return list;

            remaining -= locations.Count();
            var regions = EntityUtil.Context.Regions.Where(p => p.Description.Contains(prefixText))
                .OrderBy(p => p.Description.ToLower().IndexOf(prefixText.ToLower()))
                .Take(remaining).Select(p => p.Description).ToList().Distinct(comparer);
            list = list.Concat(regions).OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();

            if (regions.Count() == remaining) return list;

            remaining -= regions.Count();
            var address = EntityUtil.Context.Places.Where(p => p.Address.Contains(prefixText))
                .OrderBy(p => p.Address.IndexOf(prefixText))
                .Take(remaining).Select(p => p.Address).ToList().Distinct(comparer);
            return list.Concat(address).OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower())).Distinct(comparer).ToList();
        }
    }
}
