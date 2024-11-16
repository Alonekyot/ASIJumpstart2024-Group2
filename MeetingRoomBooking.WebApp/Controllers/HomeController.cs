using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Claims;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class HomeController : Controller {


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            ViewBag.ActivePage = "Dashboard";
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


        public IActionResult Bookings() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
