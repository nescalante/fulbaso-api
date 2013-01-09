using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ICourtTypeLogic
    {
        CourtType Get(int courtTypeId);

        IEnumerable<CourtType> Get(string name = null);
    }
}
