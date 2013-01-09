using System;
using System.Collections.Generic;
using System.IO;

namespace Fulbaso.Contract
{
    public interface IPlaceLogic
    {
        void Add(Place place, long userId);

        void Delete(int placeId);

        void AddImage(int placeId, Stream input, Fulbaso.Contract.File file);

        void AddImage(int placeId, string source, string description, long userId);

        void UpdateImage(int id, string text);

        void DeleteImage(int id);

        Place Get(Place place, DateTime day);

        Place Get(int placeId);

        Place Get(string page);

        Place Get(string page, DateTime day);

        /// <summary>
        /// Get page name from an id or page name
        /// </summary>
        /// <param name="page">page name</param>
        /// <returns>page name from database</returns>
        string ValidatePage(string page);

        string ValidatePage(string page, out int id);

        IEnumerable<Place> GetByUser();

        IEnumerable<Place> GetByUser(long userId);

        IEnumerable<string> GetForAutocomplete(string prefixText, int count);

        IEnumerable<Tuple<string, int>> GetTags();

        IEnumerable<Place> GetList(string value, decimal? latitude, decimal? longitude, int[] players, int[] floorTypes, string[] locations, byte[] tags, bool indoor, bool lighted, int init, int rows, out int count);

        IEnumerable<Place> GetList(string value);

        IEnumerable<Place> GetList(string value, decimal? latitude, decimal? longitude, int init, int rows, out int count);

        IEnumerable<Place> GetPendingForApproval();

        IEnumerable<Tuple<Place, double?>> GetNearest(Place place, int count = 10, double distance = 0);

        IEnumerable<Tuple<Place, double?>> GetNearest(string place, int count = 10, double distance = 0);

        IEnumerable<Tuple<Place, double?>> GetNearest(decimal? lat, decimal? lng, int count = 10, double distance = 0);

        bool PlaceHasAdmin(int placeId);

        void Update(Place place);

        bool CheckPageAvailability(string name, out string result);

        void Approve(int id);
    }
}
