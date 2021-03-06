﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    internal static class AutocompleteLogic
    {
        internal static IEnumerable<string> GetForAutocomplete(ObjectContextEntities context, string prefixText, int count)
        {
            return context.AutocompleteValues
                .Select(p => p.Value)
                .Distinct()
                .Where(p => p.Contains(prefixText))
                .OrderBy(p => p.ToLower().IndexOf(prefixText.ToLower()))
                .ThenBy(p => p.Length)
                .Take(count)
                .ToList();
        }

        internal static IEnumerable<Tuple<string, int>> GetTags(ObjectContextEntities context)
        {
            var regions = context.Places.GroupBy(p => p.Location.Region.Description).Select(p => new { p.Key, Count = p.Count(), });
            var locations = context.Places.GroupBy(p => p.Location.Description).Select(p => new { p.Key, Count = p.Count(), });

            return regions.Concat(locations).GroupBy(i => i.Key).Select(i => new { i.Key, Count = i.Sum(k => k.Count), })
                .ToList()
                .Select(i => new Tuple<string, int>(i.Key, i.Count));
        }
    }
}
