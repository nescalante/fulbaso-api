using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    public class SearchController : BaseController
    {
        private ILocationService _locationService;
        private IFloorTypeService _floorTypeService;
        private ICourtBookService _courtBookService;

        public SearchController(ILocationService locationService, IFloorTypeService floorTypeService, ICourtBookService courtBookService)
        {
            _locationService = locationService;
            _floorTypeService = floorTypeService;
            _courtBookService = courtBookService;
        }

        [HttpGet]
        public ActionResult Advanced()
        {
            ViewBag.FloorTypes = _floorTypeService.Get(string.Empty);

            return View();
        }

        [HttpPost]
        public ActionResult Advanced(FormCollection collection)
        {
            DateTime date;
            int hour;

            var players = InterfaceUtil.GetInts(collection, "player");
            var floors = InterfaceUtil.GetInts(collection, "floor");
            var locations = collection.AllKeys.Where(k => k.StartsWith("location"))
                .Select(k => collection[k]).Concat(new[] { collection["searchlocation"] }).ToArray();
            var tags = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null).Select(s => (byte)s);
            var indoor = Convert.ToBoolean(collection["indoor"].Split(',').First());
            var lighted = Convert.ToBoolean(collection["lighted"].Split(',').First());

            var dateparsed = DateTime.TryParse(collection["date"], out date);
            var hourparsed = int.TryParse(collection["hour"], out hour);

            if (hourparsed)
            {
                return RedirectToAction("Schedule", "Search", new { d = dateparsed ? date.ToShortDateString() : null, h = hourparsed ? (int?)hour : null, j = string.Join(";", players), s = string.Join(";", floors), l = string.Join(";", locations), t = string.Join(";", tags), ind = indoor, lig = lighted, });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { j = string.Join(";", players), s = string.Join(";", floors), l = string.Join(";", locations), t = string.Join(";", tags), ind = indoor, lig = lighted, });
            }
        }

        [HttpGet]
        public ActionResult Schedule(string d, int h, string q, string p, string j, string s, string l, string t, bool? ind, bool? lig)
        {
            DateTime date;
            var parsed = DateTime.TryParse(d, out date);

            date = parsed ? date : DateTime.Today;
            ViewBag.Time = date;

            int currentPage, count;
            if (p == null)
                currentPage = 1;
            else
                if (!int.TryParse(p, out currentPage))
                    currentPage = 1;

            var model = _courtBookService.GetAvailable(InterfaceUtil.GetInts(j), InterfaceUtil.GetInts(s), (l ?? "").Split(';').Where(i => !string.IsNullOrEmpty(i)).ToArray(), InterfaceUtil.GetBytes(t), ind ?? false, lig ?? false, date, h, currentPage, Configuration.RowsPerRequest, out count);
            ViewBag.Places = count;

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult FindLocation(string term)
        {
            var lista = _locationService.GetForAutocomplete(term, 10)
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
