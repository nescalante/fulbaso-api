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
        private IFavouriteService _favouriteService;
        private ILocationService _locationService;

        public PlaceController(IPlaceService placeService, ICourtService courtService, IFavouriteService favouriteService, ILocationService locationService)
        {
            _placeService = placeService;
            _courtService = courtService;
            _favouriteService = favouriteService;
            _locationService = locationService;
        }

        [HttpGet]
        //[PlaceAuthorize]
        public ActionResult Edit(string place)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                ViewBag.Locations = _locationService.Get().GetOrdered().OrderBy(l => l.Region.Description).Where(l => l.Region.IsActive).GroupBy(l => l.Region.Id).ToList();
                return View(placeModel);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [PlaceAuthorize]
        public ActionResult Edit(Place placeModel, FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["LocationJson"]))
            {
                var gr = GeocodeResponse.Get(collection["LocationJson"]);
                placeModel.Location = gr.GetLocation();
            }

            placeModel.Services = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null);
            _placeService.Update(placeModel);

            return RedirectToAction("Admin", "Home", new { place = placeModel.Page });
        }

        [HttpGet]
        public ActionResult ItemView(string place)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                placeModel.CourtsInfo = _courtService.GetByPlace(placeModel.Id);

                var model = new PlaceModel
                {
                    Place = placeModel,
                    HasAdmin = _placeService.PlaceHasAdmin(placeModel.Id),
                    IsFavourite = UserAuthentication.User != null && _favouriteService.IsFavourite(placeModel.Id, UserAuthentication.UserId),
                };

                return View("View", model);
            }
            else
                return RedirectToAction("Index", "Home");
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
            return View();
        }

        [HttpGet]
        public ActionResult Schedule(string place, string current)
        {
            DateTime date;
            var time = DateTime.TryParse(current, out date) ? date : DateTime.Today;
            var model = _placeService.Get(place, time);

            ViewBag.Time = time;

            if (model != null)
            {
                ViewBag.IsFavourite = UserAuthentication.User != null && _favouriteService.IsFavourite(model.Id, UserAuthentication.UserId);
                return View(model);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void DeleteFavourite(int favourite)
        {
            _favouriteService.Remove(favourite, UserAuthentication.UserId);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void AddFavourite(int favourite)
        {
            _favouriteService.Add(favourite, UserAuthentication.UserId);
        }
    }
}
