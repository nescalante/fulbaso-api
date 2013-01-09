using Fulbaso.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fulbaso.EntityFramework.Logic
{
    public class CourtTypeLogic : ICourtTypeLogic
    {
        private ObjectContextEntities _context;

        public CourtTypeLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public CourtType Get(int courtTypeId)
        {
            return this.Get(r => r.Id == courtTypeId).SingleOrDefault();
        }

        public IEnumerable<CourtType> Get(string name = null)
        {
            return this.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal IEnumerable<CourtType> Get(Expression<Func<CourtTypeEntity, bool>> predicate)
        {
            return _context.CourtTypes.Where(predicate).Get<CourtType, CourtTypeEntity>();
        }
    }
}
