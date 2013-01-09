using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    [Authorize]
    public class CourtController : BaseController
    {
        private ICourtLogic _courtService;
        private ICourtTypeLogic _courtTypeService;
        private IFloorTypeLogic _floorTypeService;
        private IPlaceLogic _placeService;
        
        public CourtController(ICourtLogic courtService, ICourtTypeLogic courtTypeService, IFloorTypeLogic floorTypeService, IPlaceLogic placeService)
        {
            _courtService = courtService;
            _courtTypeService = courtTypeService;
            _floorTypeService = floorTypeService;
            _placeService = placeService;
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Index(string place)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                placeModel.CourtsInfo = _courtService.GetByPlace(placeModel.Id);
                return View(placeModel);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Add(string place, bool? ft)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                ViewBag.FirstTime = ft.HasValue && ft.Value;
                ViewBag.PlacePage = placeModel.Page;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;
                ViewBag.CourtTypes = _courtTypeService.Get().Ordered();
                ViewBag.FloorTypes = _floorTypeService.Get().Ordered();

                return View("Edit");
            }
            else
                return View("Error404", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Add(Court courtModel, FormCollection collection)
        {
            courtModel.Place = Place.Create<Place>(Convert.ToInt32(collection["placeid"]));

            if (this.User is FacebookPrincipal)
            {
                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, courtModel.Place.Id)))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            
            _courtService.Add(courtModel);

            return RedirectToAction("Index", new { place = collection["placepage"] });
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Edit(string place, int court)
        {
            var placeModel = _placeService.Get(place);
            var courtModel = _courtService.Get(court);

            if (placeModel != null && courtModel != null)
            {
                ViewBag.PlacePage = placeModel.Page;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;
                ViewBag.CourtTypes = _courtTypeService.Get().Ordered();
                ViewBag.FloorTypes = _floorTypeService.Get().Ordered();

                return View("Edit", courtModel);
            }
            else
                return View("Error404", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Edit(Court courtModel, FormCollection collection)
        {
            courtModel.Place = Place.Create<Place>(Convert.ToInt32(collection["placeid"]));

            if (this.User is FacebookPrincipal)
            {
                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, courtModel.Place.Id)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            _courtService.Update(courtModel);

            return RedirectToAction("Index", new { place = collection["placepage"] });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void Delete(int id)
        {
            if (this.User is FacebookPrincipal)
            {
                var placeId = _courtService.Get(id).Place.Id;

                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, placeId)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            _courtService.Delete(id);
        }
    }
}
