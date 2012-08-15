using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Contract
{
    public interface ICourtTypeService
    {
        CourtType Get(int courtTypeId);

        IEnumerable<CourtType> Get(string name = null);
    }
}
