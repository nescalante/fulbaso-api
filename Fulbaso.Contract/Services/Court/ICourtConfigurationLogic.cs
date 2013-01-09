using System;
using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ICourtConfigurationLogic
    {
        void Add(CourtConfiguration courtConfiguration);

        void Delete(int courtConfigurationId);

        CourtConfiguration Get(int courtConfigurationId);

        IEnumerable<CourtConfiguration> GetByCourt(int courtId);

        int GetPlaceId(int id);

        decimal GetPrice(int courtId, DateTime time);

        void SetOrder(IEnumerable<int> idsInOrder);

        void Update(CourtConfiguration courtConfiguration);
    }
}
