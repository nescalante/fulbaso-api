using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Drawing;

namespace Fulbaso.Web.Controllers
{
    public class ImageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ParseUrl(string url)
        {
            var http = "http://";

            url = url.Replace(http, string.Empty);
            url = http + url;

            byte[] imageData = new byte[1];
            Uri uri;

            // changed to UriKind.Absolute to catch empty string
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        imageData = client.DownloadData(uri);
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            imageData = client.DownloadData(uri);
                            Image.FromStream(ms);

                            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
                        }
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
            else
            {
                return Json(new { status = "error", message = "URL inválida.", }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
