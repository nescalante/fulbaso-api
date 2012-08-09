using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using Fulbaso.Common;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    internal static class EntityConverter
    {
        /// <summary>
        /// Creates a new IEntity object based on EntityObject values
        /// </summary>
        /// <param name="obj">Element to get values from</param>
        /// <returns>New object based on EntityObject values</returns>
        internal static EntityDataObject ToEntity(this EntityObject obj)
        {
            return obj.ToEntity<EntityDataObject>();
        }

        /// <summary>
        /// Creates a new IEntity object based on EntityObject values
        /// </summary>
        /// <typeparam name="T">Type element to create</typeparam>
        /// <param name="obj">Element to get values from</param>
        /// <returns>New object based on EntityObject values</returns>
        internal static T ToEntity<T>(this EntityObject obj) where T : class, IEntity
        {
            if (obj == null) return null;

            // create entity with it constructor
            T entity = (T)typeof(T).GetConstructors()[0].Invoke(null);

            // set allowed types from database
            var allowedTypes = new[] { typeof(byte), typeof(byte?), typeof(int), typeof(int?), typeof(long), typeof(long?), 
                typeof(string), typeof(DateTime), typeof(DateTime?), typeof(bool), typeof(bool?), typeof(decimal), typeof(decimal?) };

            // search for the values into the entity object
            var entityProperties = obj.GetType().GetMembers().Select(m => new { m.Name, Property = m as PropertyInfo });
            var isActive = obj.GetType().GetMembers().FirstOrDefault(m => m.Name == "IsActive") as PropertyInfo;

            // search for the values into the dto object
            var objectProperties = entity.GetType().GetMembers().Where(m => entityProperties.Any(p => p.Name.ToUpper() == m.Name.ToUpper()))
                .Select(m => new { Object = m as PropertyInfo, Entity = entityProperties.First(p => p.Name.ToUpper() == m.Name.ToUpper()).Property as PropertyInfo, })
                .Where(e => e.Entity != null && e.Object != null);

            // get the entity key and assign variables
            entity.Id = Convert.ToInt32(obj.EntityKey.EntityKeyValues[0].Value);
            entity.IsActive = isActive == null || Convert.ToBoolean(isActive.GetValue(obj, null));

            // get properties to set values
            var properties = objectProperties.Where(o => allowedTypes.Contains(o.Entity.PropertyType));

            foreach (var p in properties)
                try { p.Object.SetValue(entity, p.Entity.GetValue(obj, null), null); }
                catch { }

            return entity;
        }
    }
}
