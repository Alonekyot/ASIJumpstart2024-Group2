using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Claims;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Resources.Constants;
using MeetingRoomBooking.Services.ServiceModels;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class HomeController : Controller {


        private readonly ILogger<HomeController> _logger;
        private readonly MeetingRoomBookingDbContext _context;

        public HomeController(ILogger<HomeController> logger, MeetingRoomBookingDbContext context) {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() {
            ViewBag.ActivePage = "Dashboard";

            var bookings = (from booking in _context.Bookings
                         join room in _context.Rooms on booking.RoomId equals room.RoomId
                         join user in _context.Users on booking.UserId equals user.UserId
                         select new BookingModel
                         {
                             UserName = user.FirstName + " " + user.LastName,
                             RoomName = room.RoomName,
                             MeetingDate = booking.MeetingDate,
                             StartTime = booking.StartTime,
                             EndTime = booking.EndTime,
                             RoomLocation = room.RoomLocation,
                             BookingStatus = booking.BookingStatus,
                             MeetingTitle = booking.MeetingTitle
                         }).ToList();

            return View();
        }
        public IActionResult ReportAnalytics() {
            ViewBag.ActivePage = "Report & Analytics";

            int todaysBooking = _context.Bookings
                .Where(b => b.MeetingDate == DateOnly.FromDateTime(DateTime.Now))
                .Count();
            int roomCount = _context.Rooms
                .Count();

            ViewBag.RoomCount = roomCount;
            ViewBag.TodaysBooking = todaysBooking;
            return View();
        }

        [HttpGet]
        public JsonResult GetChartData() {
            var data = new
            {
                labels = Chart.Months,
                datasets = new[]
                {
                new
                {
                    label = "Bookings",
                    data = new[] { 65, 59, 80, 81, 56, 80, 22, 48,100, 56, 32, 88 },
                    backgroundColor = Chart.BackgroundColor,
                    borderColor = Chart.BorderColor,
                    borderWidth = 1
                }
            }
            };

            return Json(data);
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
