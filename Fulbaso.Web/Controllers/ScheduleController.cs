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
        private IPlaceLogic _placeService;
        private ICourtBookLogic _courtBookService;
        private UserAuthentication _authentication;

        public ScheduleController(IPlaceLogic placeService, ICourtBookLogic courtBookService, UserAuthentication authentication)
        {
            _placeService = placeService;
            _courtBookService = courtBookService;
            _authentication = authentication;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
            bookModel.User = _authentication.GetUser().Id;

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
