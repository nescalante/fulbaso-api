namespace Fulbaso.Contract
{
    public interface IFavouriteLogic
    {
        void Add(int placeId, long userId);

        bool IsFavourite(int placeId, long userId);

        void Remove(int placeId, long userId);
    }
}
