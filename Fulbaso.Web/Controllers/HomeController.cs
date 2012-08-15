using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.EntityFramework.Logic;
using Fulbaso.Common;
using Fulbaso.Contract;
using Fulbaso.UI.Models;
using System;

namespace Fulbaso.UI.Controllers
{
    public class HomeController : BaseController
    {
        private IPlaceService _placeService;
        private IReportService _reportService;
        private UserAuthentication _authentication;

        public HomeController(IPlaceService placeService, IReportService reportService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _reportService = reportService;
            _authentication = authentication;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Admin(string place)
        {
            var model = _placeService.Get(place);

            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult List(int init, string q, string j, string s, string l, bool? ind, bool? lig)
        {
            int count;
            return View("List", GetPlacesList(init, q, j, s, l, ind, lig, out count));
        }

        [HttpGet]
        public ActionResult Index(string q, string p, string j, string s, string l, bool? ind, bool? lig)
        {
            if (j == null && s == null && l == null && ind == null && lig == null && string.IsNullOrEmpty(q))
            {
                var index = new IndexModel
                {
                    TopUsedPlaces = _reportService.GetTopVotedPlaces(2),
                    TopVotedPlaces = _reportService.GetTopUsedPlaces(2),
                    PlacesCount = _reportService.GetPlacesCount(),
                    CourtsCount = _reportService.GetCourtsCount(),
                    OwnedPlaces = _reportService.GetOwnedPlaces(),
                };

                return View(index);
            }
            else
            {
                int count;
                var model = GetPlacesList(0, q, j, s, l, ind, lig, out count);

                ViewBag.Places = count;

                if (model.Count() == 1)
                {
                    return RedirectToAction("ItemView", "Place", new { id = model.First().Page, });
                }

                return View("Places", model);
            }
        }

        [HttpPost]
        public ActionResult Index(string searchtext, FormCollection collection)
        {
            return RedirectToAction("Index", new { q = searchtext });
        }

        [HttpGet]
        public ActionResult Error404()
        {
            return View("404");
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

        private IEnumerable<Place> GetPlacesList(int init, string q, string j, string s, string l, bool? ind, bool? lig, out int count)
        {
            if (j != null || s != null || l != null || ind != null || lig != null)
            {
                return _placeService.GetList(InterfaceUtil.GetInts(j), InterfaceUtil.GetInts(s), (l ?? "").Split(';').Where(i => !string.IsNullOrEmpty(i)).ToArray(), ind ?? false, lig ?? false, init, Configuration.RowsPerRequest, out count);
            }
            else
            {
                var query = q == "*" ? string.Empty : q;
                return _placeService.GetList(query, init, Configuration.RowsPerRequest, out count);
            }
        }
    }
}
