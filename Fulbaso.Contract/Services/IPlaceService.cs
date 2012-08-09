using System;
using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IPlaceService
    {
        void Add(Place place);

        void Delete(int placeId);

        Place Get(Place place, DateTime day);

        Place Get(int placeId);

        Place Get(string page);

        Place Get(string page, DateTime day);

        IEnumerable<Place> GetByUser(long userId);

        IEnumerable<string> GetForAutocomplete(string prefixText, int count);

        IEnumerable<Place> GetList(int[] players, int[] floorTypes, string[] locations, bool indoor, bool lighted, int init, int rows, out int count);

        IEnumerable<Place> GetList(string name);

        IEnumerable<Place> GetList(string value, int init, int rows, out int count);

        IEnumerable<Tuple<Place, double?>> GetNearest(Place place, int count = 10, double distance = 0);

        bool PlaceHasAdmin(int placeId);

        void Update(Place place);
    }
}
