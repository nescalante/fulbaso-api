using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IReportService
    {
        int GetCourtsCount();

        int GetOwnedPlaces();

        int GetPlacesCount();

        IEnumerable<Place> GetTopUsedPlaces(int count);

        IEnumerable<Place> GetTopVotedPlaces(int count);
    }
}
