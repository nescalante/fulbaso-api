using System;
using System.Web.Mvc;
using Fulbaso.Common;
using Fulbaso.Common.Security;
using Fulbaso.Contract;

namespace Fulbaso.Web.Controllers
{
    [Authorize]
    public class ScheduleController : BaseController
    {
        private IPlaceService _placeService;
        private ICourtBookService _courtBookService;

        public ScheduleController(IPlaceService placeService, ICourtBookService courtBookService)
        {
            _placeService = placeService;
            _courtBookService = courtBookService;
        }

        [HttpGet]
        [PlaceAuthorize]
        public ActionResult Index(string place, string current)
        {
            DateTime date;
            var time = DateTime.TryParse(current, out date) ? date : DateTime.Today;
            var model = _placeService.Get(place, time);

            ViewBag.Time = time;

            if (model != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult EditBook(CourtBook bookModel, FormCollection collection)
        {
            var day = Convert.ToDateTime(collection["Day"]);

            bookModel.StartTime = new DateTime(day.Year, day.Month, day.Day, bookModel.StartTime.Hour, bookModel.StartTime.Minute, 0);
            bookModel.EndTime = new DateTime(day.Year, day.Month, day.Day, bookModel.EndTime.Hour, bookModel.EndTime.Minute, 0);
            bookModel.User = UserAuthentication.UserId;

            if (bookModel.Id == 0)
            {
                _courtBookService.Add(bookModel);
            }
            else
            {
                _courtBookService.Update(bookModel);
            }

            return RedirectToAction("Index", "Schedule", new { place = bookModel.Court.Place.Page, current = day.ToShortDateString(), });
        }

        [HttpPost]
        public ActionResult DeleteBook(int deleteId, string placepage, string currentday)
        {
            _courtBookService.Delete(deleteId);

            return RedirectToAction("Index", "Schedule", new { place = placepage, current = Convert.ToDateTime(currentday).ToShortDateString(), });
        }
    }
}
