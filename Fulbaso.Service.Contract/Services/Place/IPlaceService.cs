﻿using System.ServiceModel;
using System.ServiceModel.Web;

namespace Fulbaso.Service.Contract
{
    [ServiceContract]
    public interface IPlaceService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "list?term={term}&latitude={latitude}&longitude={longitude}&players={players}&floorTypes={floorTypes}&locations={locations}&tags={tags}&indoor={indoor}&lighted={lighted}&init={init}&rows={rows}")]
        PlaceListModel Get(string term, string latitude, string longitude, string players, string floorTypes, string locations, string tags, bool indoor = false, bool lighted = false, int init = 0, int rows = 10);
    }
}