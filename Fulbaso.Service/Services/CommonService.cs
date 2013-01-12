using Fulbaso.Contract;
using Fulbaso.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fulbaso.Service
{
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
