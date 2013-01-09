using Fulbaso.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fulbaso.EntityFramework.Logic
{
    public class TerritoryLogic : ITerritoryLogic
    {
        private ObjectContextEntities _context;

        public TerritoryLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public void Add(Territory territory)
        {
            var entity = new TerritoryEntity
            {
                Description = territory.Description,
                IsActive = true,
            };

            _context.Territories.AddObject(entity);
            _context.SaveChanges();

            territory.Id = entity.Id;
        }

        public Territory Get(int territoryId)
        {
            return this.Get(r => r.Id == territoryId).SingleOrDefault();
        }

        public IEnumerable<Territory> Get(string name = null)
        {
            return this.Get(c => string.IsNullOrEmpty(name) || c.Description == name);
        }

        internal IEnumerable<Territory> Get(Expression<Func<TerritoryEntity, bool>> predicate)
        {
            return _context.Territories.Where(predicate).Get<Territory, TerritoryEntity>();
        }
    }
}
