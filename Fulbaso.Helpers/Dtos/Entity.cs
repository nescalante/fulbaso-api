using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Fulbaso.Helpers
{
    /// <summary>
    /// class manages three fields: ID, Description and IsActive. Class inherits from IComparable, 
    /// so that may be used to populate the grid property AllowSorting.
    /// The method ToString() is overridden to display the Description field, or the field that is set by 
    /// the attribute PrimaryField.
    /// </summary>
    /// <typeparam name="T">Dto who inherits</typeparam>
    public class EntityDataObject : IEntity, IEntityWithId, IComparable
    {
        public static T Create<T>(int id) where T : IEntity
        {
            return EntityDataObject.Create<T>(id, string.Empty, true);
        }

        public static T Create<T>(string description) where T : IEntity
        {
            return EntityDataObject.Create<T>(default(int), description, true);
        }

        public static T Create<T>(int id, string description) where T : IEntity
        {
            return EntityDataObject.Create<T>(id, description, true);
        }

        public static T Create<T>(int id, string description, bool isActive) where T : IEntity
        {
            var entity = (T)typeof(T).GetConstructors()[0].Invoke(null);
            entity.Id = id;
            entity.Description = description;
            entity.IsActive = isActive;

            return entity;
        }

        public int Id { get; set; }

        [Display(Name = "Nombre:")]
        public virtual string Description { get; set; }

        [Display(Name = "Activo:")]
        public virtual bool IsActive { get; set; }
        public string PrimaryField
        {
            get
            {
                var primaryField = this.GetType().GetCustomAttributes(true).FirstOrDefault(a => a is PrimaryField) as PrimaryField;
                return primaryField != null ? primaryField.Field : "Description";
            }
        }

        public override string ToString()
        {
            return this.GetPrimaryField(this);
        }

        public int CompareTo(object obj)
        {
            return this.GetPrimaryField(this).CompareTo(this.GetPrimaryField(obj));
        }

        private string GetPrimaryField(object obj)
        {
            if (obj == null) return null;
            return ((this.GetType().GetMembers().First(m => m.Name == this.PrimaryField) as PropertyInfo).GetValue(obj, null) ?? string.Empty).ToString();
        }
    }
}
