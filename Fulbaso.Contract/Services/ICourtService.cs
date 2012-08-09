using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ICourtService
    {
        void Add(Court court);

        void Delete(int courtId);

        Court Get(int courtId);

        IEnumerable<Court> GetByPlace(int placeId);

        void Update(Court court);
    }
}
