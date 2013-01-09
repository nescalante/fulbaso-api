using System.Collections.Generic;

namespace Fulbaso.Contract
{
    public interface IClientLogic
    {
        void Add(Client client);

        void Delete(int clientId);

        IEnumerable<Client> GetByPlace(int placeId);

        IEnumerable<Client> GetForAutocomplete(int placeId, string text, int count);

        int GetPlaceId(int clientId);
    }
}
