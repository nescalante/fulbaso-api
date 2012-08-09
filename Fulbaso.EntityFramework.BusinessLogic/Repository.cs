using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Levex.CM.DataAccess;

namespace Levex.CM.BusinessLogic
{
    internal static class Repository<T> where T : EntityObject
    {
        static ObjectSet<T> objectSet 
        { 
            get 
            {
                return context.CreateObjectSet<T>();
            } 
        }

        static ObjectContext context
        {
            get
            {
                return EntityUtil.Context;
            }
        }

        internal static void Add(T entity)
        {
            //adds a detached object to context using entity set name and save changes
            context.AddObject(objectSet.EntitySet.Name, entity);
            context.SaveChanges();
        }

        internal static void Update(T entity)
        {
            // create the detached object's entity key.
            entity.EntityKey = context.CreateEntityKey(objectSet.EntitySet.Name, entity);
            // update entity's references 
            ApplyReferencePropertyChanges(entity);

            // call the ApplyPropertyChanges method to apply changes
            // from the updated item to the original version.
            context.ApplyCurrentValues(objectSet.EntitySet.Name, entity);
            context.SaveChanges();
        }

        internal static void Delete(T entity)
        {
            //deletes an object from its entity key
            context.DeleteObject(GetObject(entity));
            context.SaveChanges();
        }

        static T GetObject(T entity)
        {
            // Create the detached object's entity key.
            var key = objectSet.Context.CreateEntityKey(objectSet.EntitySet.Name, entity);
            var actualEntity = (T)objectSet.Context.GetObjectByKey(key);

            return actualEntity;
        }

        internal static T GetFirst(Expression<Func<T, bool>> predicate)
        {
            // gets the first item that meets the predicate
            return objectSet.First(predicate);
        }

        internal static IQueryable<T> GetQuery(Expression<Func<T, bool>> predicate)
        {
            // gets all items that meets the predicate but without querying the DB
            return objectSet.Where(predicate);
        }

        internal static IEnumerable<K> GetFromQuery<K>(IQueryable<T> query) where K : EntityDto
        {
            return query.ToList().Select(i => i.ToEntity<K>());
        }

        internal static IEnumerable<K> Get<K>(Expression<Func<T, bool>> predicate) where K : EntityDto
        {
            return GetFromQuery<K>(objectSet.Where(predicate));
        }

        internal static IEnumerable<EntityDto> Get(Expression<Func<T, bool>> predicate)
        {
            return Get<EntityDto>(predicate);
        }

        static void ApplyReferencePropertyChanges(IEntityWithRelationships entity)
        {
            // gets the entity relationships from its entity key
            var oldEntity = GetObject((T)entity) as IEntityWithRelationships;

            foreach (var relatedEnd in oldEntity.RelationshipManager.GetAllRelatedEnds())
            {
                // gets a reference from the entity on the database
                var oldRef = relatedEnd as EntityReference;

                if (oldRef != null)
                {
                    // creates a new reference object for the entity entity being updated
                    var newRef = entity.RelationshipManager.GetRelatedEnd(oldRef.RelationshipName, oldRef.TargetRoleName) as EntityReference;
                    if (newRef != null)
                    {
                        // to update the entity referenced we need its member name so it finds a member called "Value" and returns its name
                        // then we search in the entity parameters for a property named like the entity referenced
                        // (ex: if we have a Country with a Region, the entity is Country and the entity referenced is Region)
                        var entityName = (newRef.GetType().GetMembers().First(m => m.Name == "Value") as PropertyInfo).PropertyType.Name;
                        var param = Expression.Parameter(typeof(T), "tItem");
                        Type entityExprType = Expression.Property(param, entityName).Type;

                        // now with the entity referenced we must get the EntityObject. to do the cast, we use reflection to invoke 
                        // a method with the type parameters
                        Type referenceHelper = typeof(ReferenceHelper);
                        MethodInfo methodInfo = referenceHelper.GetMethod("GetReference");
                        MethodInfo genericMethod = methodInfo.MakeGenericMethod(new[] { entityExprType });

                        var entityRelated = genericMethod.Invoke(null, new Object[] { newRef });

                        // if the entity exists in the sent parameter, assign the new entity key to the entity reference on the database
                        if (entityRelated != null)
                            oldRef.EntityKey = context.CreateEntityKey(oldRef.EntityKey.EntitySetName, entityRelated);
                    }
                }
            }
        }
    }
}
