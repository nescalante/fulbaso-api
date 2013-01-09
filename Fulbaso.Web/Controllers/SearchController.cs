using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    public class SearchController : BaseController
    {
        private ILocationLogic _locationService;
        private IFloorTypeLogic _floorTypeService;
        private ICourtBookLogic _courtBookService;

        public SearchController(ILocationLogic locationService, IFloorTypeLogic floorTypeService, ICourtBookLogic courtBookService)
        {
            _locationService = locationService;
            _floorTypeService = floorTypeService;
            _courtBookService = courtBookService;
        }

        [HttpGet]
        public ActionResult Advanced()
        {
            ViewBag.FloorTypes = _floorTypeService.Get();

            return View();
        }

        [HttpPost]
        public ActionResult Advanced(FormCollection collection)
        {
            var filter = new PlacesFilter(collection);

            if (filter.IsSchedule)
            {
                return RedirectToAction("Schedule", "Search", filter.Route);
            }
            else
            {
                return RedirectToAction("Index", "Home", filter.Route);
            }
        }

        [HttpGet]
        public ActionResult Schedule(string p)
        {
            var filter = new PlacesFilter(this.Request.QueryString, true);

            ViewBag.Time = filter.Date;

            int currentPage, count;
            if (p == null)
                currentPage = 1;
            else
                if (!int.TryParse(p, out currentPage))
                    currentPage = 1;

            var model = _courtBookService.GetAvailable(filter.Players, filter.FloorTypes, filter.Locations, filter.Tags, filter.IsIndoor, filter.IsLighted, filter.Date, filter.Hour, currentPage, Configuration.RowsPerRequest, out count);
            ViewBag.Places = count;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FindLocation(string term)
        {
            var lista = _locationService.GetForAutocomplete(term, 10)
                .Select(p => new
                {
                    label = p,
                    value = p,
                    id = p,
                });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}
