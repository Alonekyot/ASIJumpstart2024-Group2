using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class BookingManagementController : Controller {
        public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
            return View();
        }

        public IActionResult Create() {
            ViewBag.ActivePage = "BookingManagement";
            return View();
        }
        public JsonResult GetEvents() {
            var events = new List<CalendarEvent>
            {
                new CalendarEvent { Title = "Event 1", Start = DateTime.Today, End = DateTime.Today.AddDays(1) },
                new CalendarEvent { Title = "Event 2", Start = new DateTime(2024, 11, 22, 8,0,0), End = new DateTime(2024, 11, 22, 12,0,0) },
            };

            return new JsonResult(events);
        }
    }
}
