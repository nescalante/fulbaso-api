using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IReportLogic
    {
        int GetCourtsCount();

        int GetOwnedPlaces();

        int GetPlacesCount();

        IEnumerable<Place> GetTopUsedPlaces(int count);

        IEnumerable<Place> GetTopVotedPlaces(int count);
    }
}
