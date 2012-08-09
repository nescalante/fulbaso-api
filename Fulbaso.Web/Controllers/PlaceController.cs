using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.EntityFramework.BusinessLogic;
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
        public ActionResult Edit(string id)
        {
            var place = CoreUtil.ValidatePlace(id);

            if (place != null)
            {
                ViewBag.Locations = _locationService.Get().GetOrdered().OrderBy(l => l.Region.Description).Where(l => l.Region.IsActive).GroupBy(l => l.Region.Id).ToList();
                return View(place);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Place place, FormCollection collection)
        {
            place.Id = Convert.ToInt32(collection["PlaceId"]);
            place.Services = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null);

            _placeService.Update(place);

            return RedirectToAction("Admin", "Home", new { id = place.Page });
        }

        [HttpGet]
        public ActionResult ItemView(string id)
        {
            var place = _placeService.Get(id);

            if (place != null)
            {
                place.CourtsInfo = _courtService.GetByPlace(place.Id);

                var model = new PlaceModel
                {
                    Place = place,
                    HasAdmin = _placeService.PlaceHasAdmin(place.Id),
                    IsFavourite = _favouriteService.IsFavourite(place.Id, FacebookLogin.Id),
                    NearPlaces = _placeService.GetNearest(place, 10, 3),
                };

                return View("View", model);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Schedule(string id, string current)
        {
            DateTime date;
            var time = DateTime.TryParse(current, out date) ? date : DateTime.Today;
            var model = _placeService.Get(id, time);

            ViewBag.Time = time;

            if (model != null)
            {
                ViewBag.IsFavourite = _favouriteService.IsFavourite(model.Id, FacebookLogin.Id);
                return View(model);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void DeleteFavourite(int favourite)
        {
            _favouriteService.Remove(favourite, FacebookLogin.Id);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public void AddFavourite(int favourite)
        {
            _favouriteService.Add(favourite, FacebookLogin.Id);
        }
    }
}
