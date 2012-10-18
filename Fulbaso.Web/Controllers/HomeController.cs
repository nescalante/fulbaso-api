using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Common;
using Fulbaso.Contract;
using Fulbaso.Web.Models;
using System;
using Fulbaso.Common.Security;

namespace Fulbaso.Web.Controllers
{
    public class HomeController : BaseController
    {
        private IPlaceService _placeService;
        private IReportService _reportService;
        private IFloorTypeService _floorTypeService;
        private UserAuthentication _authentication;

        public HomeController(IPlaceService placeService, IReportService reportService, IFloorTypeService floorTypeService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _reportService = reportService;
            _floorTypeService = floorTypeService;
            _authentication = authentication;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Admin(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        public ActionResult List(int init)
        {
            int count;
            var filter = new PlacesFilter(this.Request.QueryString);

            return View("List", GetPlacesList(init, filter, out count));
        }

        [HttpGet]
        public ActionResult Index(int? init)
        {
            var filter = new PlacesFilter(this.Request.QueryString);

            if (!filter.IsAdvanced && !filter.HasQuery)
            {
                var index = new IndexModel
                {
                    TopUsedPlaces = _reportService.GetTopVotedPlaces(4),
                    TopVotedPlaces = _reportService.GetTopUsedPlaces(4),
                    PlacesCount = _reportService.GetPlacesCount(),
                    CourtsCount = _reportService.GetCourtsCount(),
                    OwnedPlaces = _reportService.GetOwnedPlaces(),
                };

                return View(index);
            }
            else
            {
                int count;
                var model = GetPlacesList(init ?? 0, filter, out count);

                if (model.Count() == 1 && model.First().Description == filter.Query)
                {
                    return RedirectToAction("ItemView", "Place", new { place = model.First().Page, });
                }

                ViewBag.FloorTypes = _floorTypeService.Get();
                ViewBag.Places = count;

                return View("Places", model);
            }
        }

        [HttpPost]
        public ActionResult Index(string searchtext, FormCollection collection)
        {
            return RedirectToAction("Index", new { q = searchtext });
        }

        [HttpGet]
        public ActionResult Error404(string aspxerrorpath)
        {
            if (!string.IsNullOrEmpty(aspxerrorpath))
            {
                var page = _placeService.ValidatePage(aspxerrorpath.Split('/').Last());

                if (!string.IsNullOrEmpty(page))
                {
                    return RedirectToAction("ItemView", "Place", new { place = page });
                }
                else
                {
                    return RedirectToAction("Error404");
                }
            }
            else
            {
                return View("404");
            }
        }
        
        [HttpGet]
        public ActionResult LogOut()
        {
            _authentication.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(string token, FormCollection collection)
        {
            _authentication.Login(token);
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Find(string term)
        {
            var lista = _placeService.GetForAutocomplete(term, 10)
                .Select(p => new
                {
                    label = p,
                    value = p,
                    id = p,
                });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<Place> GetPlacesList(int init, PlacesFilter filter, out int count)
        {
            if (filter.IsAdvanced)
            {
                return _placeService.GetList(filter.Query, filter.Latitude, filter.Longitude, filter.Players, filter.FloorTypes, filter.Locations, filter.Tags, filter.IsIndoor, filter.IsLighted, init, Configuration.RowsPerRequest, out count);
            }
            else
            {
                return _placeService.GetList(filter.Query, filter.Latitude, filter.Longitude, init, Configuration.RowsPerRequest, out count);
            }
        }
    }
}
