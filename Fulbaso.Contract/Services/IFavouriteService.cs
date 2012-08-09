namespace Fulbaso.Contract
{
    public interface IFavouriteService
    {
        void Add(int placeId, long userId);

        bool IsFavourite(int placeId, long userId);

        void Remove(int placeId, long userId);
    }
}
