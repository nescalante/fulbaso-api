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
        private UserAuthentication _authentication;

        public HomeController(IPlaceService placeService, IReportService reportService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _reportService = reportService;
            _authentication = authentication;
        }

        [HttpGet]
        [PlaceAuthorize]
        public ActionResult Admin(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        public ActionResult List(int init, string q, string j, string s, string l, string t, bool? ind, bool? lig)
        {
            int count;
            return View("List", GetPlacesList(init, q, j, s, l, t, ind, lig, out count));
        }

        [HttpGet]
        public ActionResult Index(string q, string p, string j, string s, string l, string t, bool? ind, bool? lig)
        {
            if (j == null && s == null && l == null && ind == null && lig == null && t == null && string.IsNullOrEmpty(q))
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
                var model = GetPlacesList(0, q, j, s, l, t, ind, lig, out count);

                if (model.Count() == 1)
                {
                    return RedirectToAction("ItemView", "Place", new { place = model.First().Page, });
                }

                ViewBag.Places = count;
                ViewBag.Tags = _placeService.GetTags();

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

        private IEnumerable<Place> GetPlacesList(int init, string q, string j, string s, string l, string t, bool? ind, bool? lig, out int count)
        {
            var query = q == "*" ? string.Empty : q;

            if (j != null || s != null || l != null || ind != null || lig != null || t != null)
            {
                return _placeService.GetList(query, InterfaceUtil.GetInts(j), InterfaceUtil.GetInts(s), (l ?? "").Split(';').Where(i => !string.IsNullOrEmpty(i)).ToArray(), InterfaceUtil.GetBytes(t), ind ?? false, lig ?? false, init, Configuration.RowsPerRequest, out count);
            }
            else
            {
                return _placeService.GetList(query, init, Configuration.RowsPerRequest, out count);
            }
        }
    }
}
