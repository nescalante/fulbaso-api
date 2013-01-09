using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    [Authorize]
    public class ConfigurationController : BaseController
    {
        private ICourtConfigurationLogic _courtConfigurationService;
        private ICourtLogic _courtService;
        private IPlaceLogic _placeService;

        public ConfigurationController(ICourtConfigurationLogic courtConfigurationService, ICourtLogic courtService, IPlaceLogic placeService)
        {
            _courtConfigurationService = courtConfigurationService;
            _courtService = courtService;
            _placeService = placeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string place, int court)
        {
            var placeModel = _placeService.Get(place);
            var courtModel = _courtService.Get(court);

            if (placeModel != null && courtModel != null)
            {
                ViewBag.PlacePage = placeModel.Page;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;
                courtModel.Configuration = _courtConfigurationService.GetByCourt(court);

                return View("Index", courtModel);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add(string place, int court)
        {
            var placeModel = _placeService.Get(place);
            var courtModel = _courtService.Get(court);

            if (placeModel != null)
            {
                ViewBag.PlacePage = placeModel.Page;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;
                ViewBag.CourtId = court;
                ViewBag.CourtName = courtModel.Description;

                return View("Edit");
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpPost]
        public ActionResult Add(CourtConfiguration configModel, FormCollection collection)
        {
            configModel.Court = Court.Create<Court>(Convert.ToInt32(collection["courtid"]));
            configModel.Days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(d => collection[d.ToString()] != null);

            _courtConfigurationService.Add(configModel);

            return RedirectToAction("Index", new { place = collection["placepage"], court = configModel.Court.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string place, int court, int config)
        {
            var placeModel = _placeService.Get(place);
            var courtModel = _courtService.Get(court);
            var courtConfigModel = _courtConfigurationService.Get(config);

            if (placeModel != null && courtModel != null && courtConfigModel != null)
            {
                ViewBag.PlacePage = placeModel.Page;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;
                ViewBag.CourtId = court;
                courtConfigModel.Court = courtModel;

                return View("Edit", courtConfigModel);
            }
            else
                return RedirectToAction("Error404", "Home");
        }

        [HttpPost]
        public ActionResult Edit(CourtConfiguration configModel, FormCollection collection)
        {
            configModel.Court = Court.Create<Court>(Convert.ToInt32(collection["courtid"]));
            configModel.Days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(d => collection[d.ToString()] != null);

            _courtConfigurationService.Update(configModel);

            return RedirectToAction("Index", new { place = collection["placepage"], court = configModel.Court.Id });
        }

        [HttpPost]
        public void Delete(int id)
        {
            var user = User as FacebookPrincipal;

            if (!user.IsInRole("Admin", _courtConfigurationService.GetPlaceId(id)))
            {
                throw new UnauthorizedAccessException();
            }

            _courtConfigurationService.Delete(id);
        }

        [HttpPost]
        public void ChangeOrder(string ids)
        {
            //validar
            _courtConfigurationService.SetOrder(ids.Split(',').Select(i => Convert.ToInt32(i)));
        }
    }
}
