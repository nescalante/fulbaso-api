using System;
using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface ICourtBookService
    {
        void Add(CourtBook book);

        void Delete(int courtBookId);

        CourtBook Get(int courtBookId);

        IEnumerable<Court> GetAvailable(int[] players, int[] floorTypes, string[] locations, byte[] tags, bool indoor, bool lighted, DateTime date, int hour, int page, int rows, out int count);

        IEnumerable<CourtBook> GetByCourt(int courtId, DateTime startTime, DateTime endTime);

        IEnumerable<CourtBook> GetByPlace(int placeId, DateTime startTime, DateTime endTime);

        void Update(CourtBook book);
    }
}
