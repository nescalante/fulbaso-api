using Fulbaso.Contract;
using Fulbaso.Service.Contract;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;

namespace Fulbaso.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CommonService : ICommonService
    {
        private IPlaceLogic _placeLogic;

        public CommonService(IPlaceLogic placeLogic)
        {
            _placeLogic = placeLogic;
        }

        public List<string> GetForAutocomplete(string term)
        {
            return _placeLogic.GetForAutocomplete(term, 10).ToList();
        }
    }
}
