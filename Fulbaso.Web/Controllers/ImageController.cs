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
        private IPlaceLogic _placeService;
        private IAlbumLogic _albumService;
        private IPhotoLogic _photoService;
        private UserAuthentication _authentication;

        public ImageController(IPlaceLogic placeService, IAlbumLogic albumService, IPhotoLogic photoService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _albumService = albumService;
            _photoService = photoService;
            _authentication = authentication;
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult Index(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult FromUrl(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Editor,Admin,Owner")]
        public ActionResult FromFile(string place)
        {
            var model = _placeService.Get(place);

            return View(model);
        }

        [HttpPost]
        public ActionResult AddFromFiles(Place placeModel, FormCollection collection)
        {
            if (this.User is FacebookPrincipal)
            {
                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, placeModel.Id)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

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
                    CreatedBy = _authentication.GetUser(),
                };

                _placeService.AddImage(placeModel.Id, f.InputStream, file);
            }

            return RedirectToAction("Index", new { place = placeModel.Page, });
        }

        [HttpPost]
        public ActionResult AddFromUrls(Place placeModel, FormCollection collection)
        {
            if (this.User is FacebookPrincipal)
            {
                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, placeModel.Id)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            var list = collection.AllKeys.Where(k => k.StartsWith("src-"))
                .Select(k => Convert.ToInt32(k.Substring(4)))
                .Where(i => collection.AllKeys.Contains("desc-" + i))
                .Select(i => new { Source = collection["src-" + i], Description = collection["desc-" + i] });
            
            foreach (var i in list)
            {
                _placeService.AddImage(placeModel.Id, i.Source, i.Description, _authentication.GetUser().Id);
            }

            return RedirectToAction("Index", new { place = placeModel.Page, });
        }

        [HttpPost]
        public ActionResult DeleteImage(int id, string page, FormCollection collection)
        {
            if (this.User is FacebookPrincipal)
            {
                var placeId = _placeService.Get(page).Id;

                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, placeId)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            _placeService.DeleteImage(id);

            return RedirectToAction("Index", new { place = page, });
        }

        [HttpPost]
        public ActionResult UpdateImage(int id, string text, string page, FormCollection collection)
        {
            if (this.User is FacebookPrincipal)
            {
                var placeId = _placeService.Get(page).Id;

                var fp = this.User as FacebookPrincipal;
                if (!new[] { "Editor", "Admin", "Owner" }.Any(r => fp.IsInRole(r, placeId)))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            _placeService.UpdateImage(id, text);

            return RedirectToAction("Index", new { place = page, });
        }

        [HttpGet]
        [Authorize]
        public ActionResult ParseUrl(string url)
        {
            try
            {
                ImageHelper.GetImageFromUrl(url);

                return Json(new { status = "ok", }, JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentException)
            {
                if (url.ToLower().Contains("facebook.com"))
                {
                    try
                    {
                        var albums = _albumService.Get(url.Split('/').Last().Split('?').First());
                        var photos = albums.SelectMany(a => _photoService.GetFromAlbum(a.Id));

                        return Json(new { status = "facebook", content = photos, }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { status = "error", message = "No se pudo encontrar el recurso.", }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    throw new WebException();
                }
            }
            catch (WebException)
            {
                return Json(new { status = "error", message = "URL inválida.", }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
