using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.UI.Controllers
{
    [Authorize]
    public class ConfigurationController : BaseController
    {
        private ICourtConfigurationService _courtConfigurationService;
        private ICourtService _courtService;

        public ConfigurationController(ICourtConfigurationService courtConfigurationService, ICourtService courtService)
        {
            _courtConfigurationService = courtConfigurationService;
            _courtService = courtService;
        }

        [HttpGet]
        public ActionResult Index(string id, int court)
        {
            var place = CoreUtil.ValidatePlace(id);
            var courtModel = _courtService.Get(court);

            if (place != null && courtModel != null)
            {
                ViewBag.PlacePage = place.Page;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;
                courtModel.Configuration = _courtConfigurationService.GetByCourt(court);

                return View("Index", courtModel);
            }
            else
                return View("Index", "Home");
        }

        [HttpGet]
        public ActionResult Add(string id, int court)
        {
            var place = CoreUtil.ValidatePlace(id);
            var courtModel = _courtService.Get(court);

            if (place != null)
            {
                ViewBag.PlacePage = place.Page;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;
                ViewBag.CourtId = court;
                ViewBag.CourtName = courtModel.Description;

                return View("Edit");
            }
            else
                return View("Index", "Home");
        }

        [HttpPost]
        public ActionResult Add(CourtConfiguration config, FormCollection collection)
        {
            config.Court = Court.Create<Court>(Convert.ToInt32(collection["courtid"]));
            config.Days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(d => collection[d.ToString()] != null);

            _courtConfigurationService.Add(config);

            return RedirectToAction("Index", new { id = collection["placepage"], court = config.Court.Id });
        }

        [HttpGet]
        public ActionResult Edit(string id, int court, int config)
        {
            var place = CoreUtil.ValidatePlace(id);
            var courtModel = _courtService.Get(court);
            var courtConfigModel = _courtConfigurationService.Get(config);

            if (place != null && courtModel != null && courtConfigModel != null)
            {
                ViewBag.PlacePage = place.Page;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;
                ViewBag.CourtId = court;
                courtConfigModel.Court = courtModel;

                return View("Edit", courtConfigModel);
            }
            else
                return View("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(CourtConfiguration config, FormCollection collection)
        {
            config.Court = Court.Create<Court>(Convert.ToInt32(collection["courtid"]));
            config.Days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(d => collection[d.ToString()] != null);

            _courtConfigurationService.Update(config);

            return RedirectToAction("Index", new { id = collection["placepage"], court = config.Court.Id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void Delete(int id)
        {
            if (!User.GetPlaces().Any(up => up.Id == _courtConfigurationService.GetPlaceId(id)))
            {
                throw new UnauthorizedAccessException();
            }

            _courtConfigurationService.Delete(id);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ChangeOrder(string ids)
        {
            //validar
            _courtConfigurationService.SetOrder(ids.Split(',').Select(i => Convert.ToInt32(i)));
        }
    }
}
