using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Common;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    public class CourtTypeService : ICourtTypeService
    {
        public EntityDataObject Get(int courtTypeId)
        {
            return CourtTypeService.Get(r => r.Id == courtTypeId).Single();
        }

        public IEnumerable<EntityDataObject> Get(string name = null)
        {
            return CourtTypeService.Get(c => string.IsNullOrEmpty(name) || c.Description.Contains(name));
        }

        internal static IEnumerable<EntityDataObject> Get(Expression<Func<CourtTypeEntity, bool>> predicate)
        {
            return Repository<CourtTypeEntity>.Get(predicate);
        }
    }
}
