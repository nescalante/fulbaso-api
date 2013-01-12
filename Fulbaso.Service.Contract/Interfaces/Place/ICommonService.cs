﻿using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Fulbaso.Service.Contract
{
    [ServiceContract]
    public interface ICommonService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "autocomplete?term={term}")]
        List<string> GetForAutocomplete(string term);
    }
}