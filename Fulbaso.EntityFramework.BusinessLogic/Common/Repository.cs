using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fulbaso.Helpers;

namespace Fulbaso.EntityFramework.Logic
{
    internal static class RepositoryExtensions
    {
        internal static IEnumerable<K> Get<K, T>(this IQueryable<T> query) where K : EntityDataObject where T : EntityObject
        {
            return query.ToList().Select(i => i.ToEntity<K>()).ToList();
        }
    }
}