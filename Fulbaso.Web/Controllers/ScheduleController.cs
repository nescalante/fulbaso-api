using System;
using System.Web.Mvc;
using Fulbaso.EntityFramework.BusinessLogic;
using Fulbaso.Common;
using Fulbaso.Contract;

namespace Fulbaso.UI.Controllers
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
        public ActionResult Index(string id, string current)
        {
            var place = CoreUtil.ValidatePlace(id);
            if (place == null) throw new UnauthorizedAccessException();

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
        public ActionResult EditBook(CourtBook book, FormCollection collection)
        {
            var day = Convert.ToDateTime(collection["Day"]);

            book.StartTime = new DateTime(day.Year, day.Month, day.Day, book.StartTime.Hour, book.StartTime.Minute, 0);
            book.EndTime = new DateTime(day.Year, day.Month, day.Day, book.EndTime.Hour, book.EndTime.Minute, 0);
            book.User = FacebookLogin.Id;

            if (book.Id == 0)
            {
                _courtBookService.Add(book);
            }
            else
            {
                _courtBookService.Update(book);
            }

            return RedirectToAction("Index", "Schedule", new { id = book.Court.Place.Page, current = day.ToShortDateString(), });
        }

        [HttpPost]
        public ActionResult DeleteBook(int deleteId, string placepage, string currentday)
        {
            _courtBookService.Delete(deleteId);

            return RedirectToAction("Index", "Schedule", new { id = placepage, current = Convert.ToDateTime(currentday).ToShortDateString(), });
        }
    }
}
