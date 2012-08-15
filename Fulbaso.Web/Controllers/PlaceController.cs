using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Common;
using Fulbaso.Contract;
using Fulbaso.UI.Models;

namespace Fulbaso.UI.Controllers
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
        [Authorize]
        public ActionResult Edit(string place)
        {
            var placeModel = CoreUtil.ValidatePlace(place);

            if (placeModel != null)
            {
                ViewBag.Locations = _locationService.Get().GetOrdered().OrderBy(l => l.Region.Description).Where(l => l.Region.IsActive).GroupBy(l => l.Region.Id).ToList();
                return View(placeModel);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Place placeModel, FormCollection collection)
        {
            placeModel.Id = Convert.ToInt32(collection["PlaceId"]);
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
                    NearPlaces = _placeService.GetNearest(placeModel, 10, 3),
                };

                return View("View", model);
            }
            else
                return RedirectToAction("Index", "Home");
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
