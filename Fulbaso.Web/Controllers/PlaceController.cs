using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;
using Fulbaso.Web.Models;
using System.Collections.Generic;
using Fulbaso.Helpers;

namespace Fulbaso.Web.Controllers
{
    public class PlaceController : BaseController
    {
        private IPlaceService _placeService;
        private ICourtService _courtService;
        private ICourtTypeService _courtTypeService;
        private IFavouriteService _favouriteService;
        private IFloorTypeService _floorTypeService;
        private ILocationService _locationService;
        private ITerritoryService _territoryService;
        private UserAuthentication _authentication;

        public PlaceController(IPlaceService placeService, ICourtService courtService, ICourtTypeService courtTypeService, IFavouriteService favouriteService, IFloorTypeService floorTypeService, ILocationService locationService, ITerritoryService territoryService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _courtService = courtService;
            _courtTypeService = courtTypeService;
            _favouriteService = favouriteService;
            _floorTypeService = floorTypeService;
            _locationService = locationService;
            _territoryService = territoryService;
            _authentication = authentication;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(Place placeModel, FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["LocationJson"]))
            {
                var gr = GeocodeResponse.Get(collection["LocationJson"]);
                placeModel.Location = gr.GetLocation();
            }

            placeModel.Services = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null);
            _placeService.Update(placeModel);

            return RedirectToAction("Edit", "Place", new { place = placeModel.Page });
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(string place)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                ViewBag.Locations = _locationService.Get().Ordered().OrderBy(l => l.Region.Description).Where(l => l.Region.IsActive).GroupBy(l => l.Region.Id).ToList();
                return View(placeModel);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(Place placeModel, FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["LocationJson"]))
            {
                var gr = GeocodeResponse.Get(collection["LocationJson"]);
                placeModel.Location = gr.GetLocation();
            }

            placeModel.Services = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null);
            _placeService.Update(placeModel);

            return RedirectToAction("Edit", "Place", new { place = placeModel.Page });
        }

        [HttpGet]
        public ActionResult ItemView(string place, string current)
        {
            var placeModel = _placeService.Get(place);
            DateTime date;
            var day = DateTime.TryParse(current, out date) ? date.Date : DateTime.Today;

            if (placeModel != null)
            {
                placeModel.CourtsInfo = _courtService.GetByPlace(placeModel.Id);

                var user = _authentication.GetUser();
                var model = new PlaceModel
                {
                    Place = placeModel,
                    HasAdmin = _placeService.PlaceHasAdmin(placeModel.Id),
                    IsFavourite = user != null && _favouriteService.IsFavourite(placeModel.Id, user.Id),
                };

                if (model.HasAdmin)
                {
                    model.Place = _placeService.Get(placeModel, day);
                }

                return View("View", model);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpGet]
        public ActionResult GetNearest(string place)
        {
            return Json(_placeService.GetNearest(place, 0, 30).WithUrl(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNearestLayout(string place, string description)
        {
            ViewBag.Description = description;
            return View("NearPlaces", _placeService.GetNearest(place, 5).WithUrl());
        }

        [HttpGet]
        public ActionResult GetNearestFromLocation(decimal lat, decimal lng, double distance = 30)
        {
            return Json(_placeService.GetNearest(lat, lng, 0, distance).WithUrl(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNearestList(decimal lat, decimal lng)
        {
            Position.Set(lat, lng);

            return View("List", _placeService.GetNearest(lat, lng, 10).Select(i => i.Item1).WithUrl());
        }

        [HttpGet]
        public ActionResult InfoView(Place placeModel)
        {
            return View(placeModel);
        }

        [HttpGet]
        public ActionResult Map()
        {
            ViewBag.FloorTypes = _floorTypeService.Get();

            return View();
        }

        [HttpGet]
        public ActionResult List()
        {
            var filter = new PlacesFilter(this.Request.QueryString);
            int count;

            if (filter.IsAdvanced)
            {
                return Json(_placeService.GetList(filter.Query, filter.Latitude, filter.Longitude, filter.Players, filter.FloorTypes, filter.Locations, filter.Tags, filter.IsIndoor, filter.IsLighted, 0, 0, out count).WithUrl(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_placeService.GetList(filter.Query, filter.Latitude, filter.Longitude, 0, 0, out count).WithUrl(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Schedule(string place, string current, bool? layout)
        {
            DateTime date;
            var time = DateTime.TryParse(current, out date) ? date : DateTime.Today;
            var model = _placeService.Get(place, time);

            ViewBag.NullLayout = layout;
            ViewBag.Time = time;

            if (model != null)
            {
                return View(model);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void DeleteFavourite(int favourite)
        {
            _favouriteService.Remove(favourite, _authentication.GetUser().Id);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void AddFavourite(int favourite)
        {
            _favouriteService.Add(favourite, _authentication.GetUser().Id);
        }
    }
}
