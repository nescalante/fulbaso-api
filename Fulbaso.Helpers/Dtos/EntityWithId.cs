using System;

namespace Fulbaso.Helpers
{
    [Serializable]
    public abstract class EntityWithId : IEntityWithId
    {
        public static T Create<T>(int id) where T : IEntityWithId
        {
            var entity = (T)typeof(T).GetConstructors()[0].Invoke(null);
            entity.Id = id;

            return entity;
        }

        public int Id { get; set; }
    }
}
