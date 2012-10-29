using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;
using Fulbaso.Web.Models;
using Fulbaso.Helpers;

namespace Fulbaso.Web.Controllers
{
    public class HomeController : BaseController
    {
        private IPlaceService _placeService;
        private IReportService _reportService;
        private IFloorTypeService _floorTypeService;
        private ILocationService _locationService;
        private ITerritoryService _territoryService;
        private UserAuthentication _authentication;

        public HomeController(IPlaceService placeService, IReportService reportService, IFloorTypeService floorTypeService, ILocationService locationService, ITerritoryService territoryService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _reportService = reportService;
            _floorTypeService = floorTypeService;
            _locationService = locationService;
            _territoryService = territoryService;
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
                ViewBag.PossibleLocations = GetPossibleLocations(filter);

                return View("Places", model);
            }
        }

        private List<string> GetPossibleLocations(PlacesFilter filter)
        {
            var locations = new List<string>();

            foreach (var l in filter.Locations)
            {
                locations.AddRange(_locationService.GetRelated(l));
            }

            if (Position.Location != null)
            {
                locations.AddRange(_locationService.GetRelated(Position.Location.Description));
                locations.Add(Position.Location.Description);
                locations.Add(Position.Location.Region.Description);
            }

            if (!locations.Any())
            {
                locations.AddRange(_territoryService.Get().Select(t => t.Description));
            }

            return locations.Distinct(new CaseInsensitiveComparer()).Where(l => !string.IsNullOrEmpty(l)).ToList();
        }

        [HttpPost]
        public ActionResult Index(string searchtext, FormCollection collection)
        {
            return RedirectToAction("Index", new { q = string.IsNullOrEmpty(searchtext) ? "*" : searchtext });
        }

        [HttpGet]
        public ActionResult Error404(string aspxerrorpath, string returnUrl)
        {
            throw new HttpException((int)HttpStatusCode.NotFound, "");
        }
        
        [HttpGet]
        public ActionResult LogOut()
        {
            var url = string.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", Url.Action("Index", "Home", null, "http"), _authentication.GetUser().Token);
            _authentication.Logout();

            return Redirect(url);
        }

        [HttpPost]
        public ActionResult Login(string token, FormCollection collection)
        {
            _authentication.Login(token);
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [HttpGet]
        public ActionResult GetToken(string code, string state)
        {
            try
            {
                var tokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}", Configuration.AppId, Configuration.AppSecret, Url.Action("GetToken", "Home", null, "http"), code);
                var request = WebRequest.Create(tokenUrl) as HttpWebRequest;
                var token = request.GetString();
                var qs = HttpUtility.ParseQueryString(token);

                _authentication.Login(qs["access_token"]);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Could not authorize user.", ex);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LogIn(string lat, string lng)
        {
            decimal dlat, dlng;

            if (decimal.TryParse(lat, out dlat) && decimal.TryParse(lng, out dlng))
            {
                Position.Set(dlat, dlng);
            }

            var url = "https://www.facebook.com/dialog/oauth?client_id=" + Configuration.AppId + 
                      "&redirect_uri=" + Url.Action("GetToken", "Home", null, "http") + 
                      "&scope=email,user_about_me,user_birthday,user_hometown,user_photos,friends_about_me,offline_access,create_event&state=" + Guid.NewGuid();

            return Redirect(url);
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
