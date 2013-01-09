using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    [Authorize]
    public class ClientController : BaseController
    {
        private IClientLogic _clientService;
        private IPlaceLogic _placeService;

        public ClientController(IClientLogic clientService, IPlaceLogic placeService)
        {
            _clientService = clientService;
            _placeService = placeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string place)
        {
            var placeModel = _placeService.Get(place);

            if (placeModel != null)
            {
                ViewBag.PlacePage = place;
                ViewBag.Place = placeModel.Description;
                ViewBag.PlaceId = placeModel.Id;

                var model = _clientService.GetByPlace(placeModel.Id);

                return View(model);
            }
            else
                return View("Index", "Home");
        }

        [HttpPost]
        public void Delete(int id)
        {
            var user = User as FacebookPrincipal;

            if (!user.IsInRole("Admin", _clientService.GetPlaceId(id)))
            {
                throw new UnauthorizedAccessException();
            }

            _clientService.Delete(id);
        }

        [HttpGet]
        public JsonResult Find(string term, int place)
        {
            var list = _clientService.GetForAutocomplete(place, term, 10)
                .Select(p => new
                {
                    label = p.Description + (string.IsNullOrEmpty(p.Phone) ? string.Empty : (" (" + p.Phone + ")")),
                    value = p.Description + (string.IsNullOrEmpty(p.Phone) ? string.Empty : (" (" + p.Phone + ")")),
                    id = p.Id,
                    name = p.Description,
                    phone = p.Phone,
                });

            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
