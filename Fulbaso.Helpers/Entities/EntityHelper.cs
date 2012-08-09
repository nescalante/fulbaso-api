using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;

namespace Fulbaso.Helpers
{
    public static class EntityHelper
    {
        /// <summary>
        /// Include extension method for ObjectQuery
        /// </summary>
        /// <typeparam name="T">Type of elements in IQueryable</typeparam>
        /// <param name="queryable">Queryable object</param>
        /// <param name="path">Expression with path to include</param>
        /// <returns>Queryable object with include path information</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ObjectQuery<T> Include<T>(this IQueryable<T> queryable, Expression<Func<T, object>> path)
            where T : class,new()
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var body = path.Body as MemberExpression;
            if ((body == null || !body.Member.DeclaringType.IsAssignableFrom(typeof(T))) || body.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("path");

            return ((ObjectQuery<T>)queryable).Include(body.Member.Name);
        }

        public static string ToTraceString<T>(this IQueryable<T> query)
        {
            if (query is ObjectQuery)
                return ((ObjectQuery)query).ToTraceString();
            return query.ToString();
        }

        public static void SetAllModified<T>(this ObjectContext context, T entity) where T : IEntityWithKey
        {
            // sets the modified state property for all properties of the entity
            var stateEntry = context.ObjectStateManager.GetObjectStateEntry(entity.EntityKey);
            var propertyNameList = stateEntry.CurrentValues.DataRecordInfo.FieldMetadata.Select(pn => pn.FieldType.Name);
            foreach (var propName in propertyNameList)
            {
                stateEntry.SetModifiedProperty(propName);
            }
        }

        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(
            Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }

            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate(Expression.Or);

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static IQueryable<TElement> WhereContains<TElement, TValue>(
            this IQueryable<TElement> query, Expression<Func<TElement, TValue>> selector, IEnumerable<TValue> values)
        {
            if (null == selector) { throw new ArgumentNullException("selector"); }
            if (null == values) { throw new ArgumentNullException("values"); }

            ParameterExpression p = selector.Parameters.Single();

            if (!values.Any())
            {
                return query.Where(q => false);
            }

            var equals = values.Select(value => (Expression)Expression.Equal(selector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate(Expression.Or);

            var expression = Expression.Lambda<Func<TElement, bool>>(body, p);

            return query.Where(expression);
        }

        private static Expression<Func<TElement, bool>> GetWhereNotInExpression<TElement, TValue>(
            Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            ParameterExpression p = propertySelector.Parameters.Single();

            if (!values.Any())
            {
                return e => true;
            }

            var unequals = values.Select(value =>
                (Expression)Expression.NotEqual(propertySelector.Body,Expression.Constant(value, typeof(TValue))));

            var body = unequals.Aggregate(Expression.And);

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(
            this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
        {
            return source.Where(EntityHelper.GetWhereNotInExpression(propertySelector, values));
        }

        public static IQueryable<TElement> WhereNotIn<TElement, TValue>(
            this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
        {
            return source.Where(EntityHelper.GetWhereNotInExpression(propertySelector, values));
        }
    }
}
