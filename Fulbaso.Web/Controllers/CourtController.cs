using System;
using System.Web.Mvc;
using Fulbaso.EntityFramework.BusinessLogic;
using Fulbaso.Contract;

namespace Fulbaso.UI.Controllers
{
    [Authorize]
    public class CourtController : BaseController
    {
        private ICourtService _courtService;
        private ICourtTypeService _courtTypeService;
        private IFloorTypeService _floorTypeService;
        
        public CourtController(ICourtService courtService, ICourtTypeService courtTypeService, IFloorTypeService floorTypeService)
        {
            _courtService = courtService;
            _courtTypeService = courtTypeService;
            _floorTypeService = floorTypeService;
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            var place = CoreUtil.ValidatePlace(id);

            if (place != null)
            {
                place.CourtsInfo = _courtService.GetByPlace(place.Id);
                return View(place);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Add(string id)
        {
            var place = CoreUtil.ValidatePlace(id);

            if (place != null)
            {
                ViewBag.PlacePage = place.Page;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;
                ViewBag.CourtTypes = _courtTypeService.Get().GetOrdered();
                ViewBag.FloorTypes = _floorTypeService.Get().GetOrdered();

                return View("Edit");
            }
            else
                return View("Index", "Home");
        }

        [HttpPost]
        public ActionResult Add(Court court, FormCollection collection)
        {
            court.Place = Place.Create<Place>(Convert.ToInt32(collection["placeid"]));
            _courtService.Add(court);

            return RedirectToAction("Index", new { id = collection["placepage"] });
        }

        [HttpGet]
        public ActionResult Edit(string id, int court)
        {
            var place = CoreUtil.ValidatePlace(id);
            var courtModel = _courtService.Get(court);

            if (place != null && courtModel != null)
            {
                ViewBag.PlacePage = place.Page;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;
                ViewBag.CourtTypes = _courtTypeService.Get().GetOrdered();
                ViewBag.FloorTypes = _floorTypeService.Get().GetOrdered();

                return View("Edit", courtModel);
            }
            else
                return View("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(Court court, FormCollection collection)
        {
            court.Place = Place.Create<Place>(Convert.ToInt32(collection["placeid"]));

            _courtService.Update(court);

            return RedirectToAction("Index", new { id = collection["placepage"] });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void Delete(int id)
        {
            // validar
            _courtService.Delete(id);
        }
    }
}
