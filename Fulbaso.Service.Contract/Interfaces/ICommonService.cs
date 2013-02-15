using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Fulbaso.Service.Contract
{
    [ServiceContract]
    public interface ICommonService
    {
        /// <summary>
        /// Return the suggestions for the term
        /// </summary>
        /// <param name="term">some term</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
                    ResponseFormat = WebMessageFormat.Json,
                    BodyStyle = WebMessageBodyStyle.Bare,
                    UriTemplate = "autocomplete?term={term}")]
        List<string> GetForAutocomplete(string term);
    }
}
