﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.EntityFramework.BusinessLogic;
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

        public HomeController(IPlaceService placeService, IReportService reportService)
        {
            _placeService = placeService;
            _reportService = reportService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Admin(string id)
        {
            var model = _placeService.Get(id);

            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult List(int init, string query)
        {
            int count;
            var model = _placeService.GetList(query == "*" ? string.Empty : query, init, Configuration.RowsPerRequest, out count);

            return View("List", model);
        }

        [HttpGet]
        public ActionResult Search(string query)
        {
            query = query == "*" ? string.Empty : query;
            int count;
            var model = _placeService.GetList(query, 0, Configuration.RowsPerRequest, out count);
            ViewBag.Query = query;
            ViewBag.Count = count;

            if (count == 1)
            {
                return RedirectToAction("ItemView", "Place", new { id = model.First().Page, });
            }

            return View("Places", model);
        }

        [HttpGet]
        public ActionResult Index(string q, string p, string j, string s, string l, bool? ind, bool? lig)
        {
            int currentPage, count;
            if (p == null)
                currentPage = 1;
            else
                if (!int.TryParse(p, out currentPage))
                    currentPage = 1;
            IEnumerable<Place> model;

            if (j != null || s != null || l != null || ind != null || lig != null)
            {
                model = _placeService.GetList(InterfaceUtil.GetInts(j), InterfaceUtil.GetInts(s), (l ?? "").Split(';').Where(i => !string.IsNullOrEmpty(i)).ToArray(), ind ?? false, lig ?? false, currentPage, Configuration.RowsPerRequest, out count);
                ViewBag.Places = count;
            }
            else
            {
                if (string.IsNullOrEmpty(q))
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

                var query = q == "*" ? string.Empty : q;
                model = _placeService.GetList(query, currentPage, Configuration.RowsPerRequest, out count);
                ViewBag.Places = count;
                ViewBag.Query = query;

                if (model.Count() == 1 && currentPage == 1)
                {
                    return RedirectToAction("ItemView", "Place", new { id = model.First().Page, });
                }
            }
            
            return View("Places", model);
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
            FacebookLogin.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(string token, FormCollection collection)
        {
            FacebookLogin.Login(token);
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
    }
}
