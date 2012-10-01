using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    public class ImageController : BaseController
    {
        private IPlaceService _placeService;

        public ImageController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        [PlaceAuthorize]
        public ActionResult Index(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        [PlaceAuthorize]
        public ActionResult FromUrl(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        [PlaceAuthorize]
        public ActionResult FromFile(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddFromFiles(Place placeModel, FormCollection collection)
        {
            if (!User.HasPlace(placeModel.Id)) throw new UnauthorizedAccessException();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var f = Request.Files[i];
                var desc = "desc-" + i;
                var file = new File
                {
                    ContentLength = f.ContentLength,
                    ContentType = f.ContentType,
                    FileName = f.FileName,
                    Description = collection.AllKeys.Contains(desc) ? collection[desc] : null,
                    CreatedBy = UserAuthentication.User,
                };

                _placeService.AddImage(placeModel.Id, f.InputStream, file);
            }

            return RedirectToAction("Index", new { place = placeModel.Page, });
        }

        [HttpGet]
        public ActionResult ParseUrl(string url)
        {
            try
            {
                ImageUtil.GetImageFromUrl(url);

                return Json(new { status = "ok", }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentException ex)
            {
                return Json(new { status = "error", message = "No se pudo encontrar la imagen.", }, JsonRequestBehavior.AllowGet);
            }
            catch (WebException ex)
            {
                return Json(new { status = "error", message = "URL inválida.", }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
