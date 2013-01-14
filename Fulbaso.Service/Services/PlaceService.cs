using Fulbaso.Contract;
using Fulbaso.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;

namespace Fulbaso.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PlaceService : IPlaceService
    {
        private IPlaceLogic _placeLogic;
        private ICourtLogic _courtLogic;

        public PlaceService(IPlaceLogic placeLogic, ICourtLogic courtLogic)
        {
            _placeLogic = placeLogic;
            _courtLogic = courtLogic;
        }

        public PlaceListModel List(string term, string latitude, string longitude, string players, string floorTypes, string locations, string tags, bool indoor = false, bool lighted = false, int init = 0, int rows = 10)
        {
            IEnumerable<Place> list;
            int count;

            var filter = new Filter(term, latitude.GetDecimal(), longitude.GetDecimal(), players.GetInts('-'), floorTypes.GetInts('-'), (locations ?? "").Split('-'), tags.GetBytes('-'), indoor, lighted);

            if (filter.IsAdvanced)
            {
                list = _placeLogic.GetList(null, filter.Latitude, filter.Longitude, filter.Players, filter.FloorTypes, filter.Locations, filter.Tags, filter.IsIndoor, filter.IsLighted, init, rows, out count);
            }
            else
            {
                list = _placeLogic.GetList(filter.Query, filter.Latitude, filter.Longitude, init, rows, out count);
            }

            return new PlaceListModel
            {
                Count = count,
                List = list.Select(i => (PlaceListItemModel)i).ToList(),
                Title = filter.Reference,
                Description = filter.ToString(),
            };
        }

        public PlaceModel Get(string id)
        {
            var model = _placeLogic.Get(id);

            if (model != null)
            {
                model.CourtsInfo = _courtLogic.GetByPlace(model.Id);

                return (PlaceModel)model;
            }

            throw new Exception("place not found"); ;
        }
    }
}
