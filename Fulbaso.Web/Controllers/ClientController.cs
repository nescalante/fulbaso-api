﻿using System;
using System.Linq;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.UI.Controllers
{
    [Authorize]
    public class ClientController : BaseController
    {
        private IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            var place = CoreUtil.ValidatePlace(id);

            if (place != null)
            {
                ViewBag.PlacePage = id;
                ViewBag.Place = place.Description;
                ViewBag.PlaceId = place.Id;

                var model = _clientService.GetByPlace(place.Id);

                return View(model);
            }
            else
                return View("Index", "Home");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void Delete(int id)
        {
            if (!User.GetPlaces().Any(up => up.Id == _clientService.GetPlaceId(id)))
            {
                throw new UnauthorizedAccessException();
            }

            _clientService.Delete(id);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Find(string term, int place)
        {
            var lista = _clientService.GetForAutocomplete(place, term, 10)
                .Select(p => new
                {
                    label = p.Description + (string.IsNullOrEmpty(p.Phone) ? string.Empty : (" (" + p.Phone + ")")),
                    value = p.Description + (string.IsNullOrEmpty(p.Phone) ? string.Empty : (" (" + p.Phone + ")")),
                    id = p.Id,
                    name = p.Description,
                    phone = p.Phone,
                });

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}