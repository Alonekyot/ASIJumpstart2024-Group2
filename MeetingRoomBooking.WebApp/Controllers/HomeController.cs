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
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.Interfaces;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class HomeController : Controller {

        private readonly IBookingManager _bookingManager;
        private readonly ILogger<HomeController> _logger;
        private readonly MeetingRoomBookingDbContext _context;
        private readonly ChartDataManager _chartDataManager;
        public HomeController(ILogger<HomeController> logger, MeetingRoomBookingDbContext context, IBookingManager bookingManager, ChartDataManager chartDataManager) {
            _logger = logger;
            _context = context;
            _chartDataManager = chartDataManager;
            _bookingManager = bookingManager;
        }

        public IActionResult Index() {

            ViewBag.ActivePage = "Dashboard";
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var role = int.Parse(User.FindFirstValue("Role"));

            if (role == 1 || role == 2)
            {
                // For Admin: fetch all bookings (both admin and user)
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
                                 MeetingTitle = booking.MeetingTitle,
                                 BookingID = booking.BookingId
                             }).ToList();

                
                return View("AdminDashboard", bookings); // Pass the bookings to the Admin view
            }
            else if (role == 0)
            {
                // For User: fetch bookings only for the logged-in user
                var bookings = (from booking in _context.Bookings
                                join room in _context.Rooms on booking.RoomId equals room.RoomId
                                where booking.UserId == userId
                                where booking.BookingStatus != "Canceled"
                                select new BookingModel
                                {                                  
                                    RoomName = room.RoomName,
                                    MeetingDate = booking.MeetingDate,
                                    StartTime = booking.StartTime,
                                    EndTime = booking.EndTime,
                                    RoomLocation = room.RoomLocation,
                                    BookingStatus = booking.BookingStatus,
                                    MeetingTitle = booking.MeetingTitle,
                                    BookingID = booking.BookingId
                                }).ToList();

                return View("UserDashboard", bookings); // Pass the bookings to the User view
            }
            return View();

        }
        public IActionResult ReportAnalytics() {
            ViewBag.ActivePage = "Report & Analytics";

            int todaysBooking = _context.Bookings
                .Where(b => b.MeetingDate == DateOnly.FromDateTime(DateTime.Now))
                .Count();
            int roomCount = _context.Rooms
                .Count();
            int recurring = _context.Bookings
                .Where(b => b.Recurring)
                .Count();

            var roomLeaderboard = _context.Rooms
                .Select(room => new RoomLeaderBoard {
                    RoomName = room.RoomName,
                    BookedTimes = room.booking.Count()
                })
                .OrderByDescending(x => x.BookedTimes)
                .ToList();

            var userLeaderboard = _context.Users
                .Where(u => !u.Deleted)
                .Select(user => new UserLeaderBoard
                {
                    Username = user.FirstName + " " + user.LastName,
                    BookedTimes = user.booking.Count()
                })
                .Where(s => s.BookedTimes > 0)
                .OrderByDescending(u => u.BookedTimes)
                .ToList();
            var leaderboards = new LeaderBoardLists()
            {
                RoomLeaders = roomLeaderboard,
                UserLeaders = userLeaderboard
            };

            ViewBag.RoomCount = roomCount;
            ViewBag.TodaysBooking = todaysBooking;
            ViewBag.Recurrings = recurring;
            return View(leaderboards);
        }

        [HttpGet]
        public JsonResult GetChartData() {
            var dataSet = _chartDataManager.GetBarChartData();
            var data = new
            {
                labels = Chart.Months,
                datasets = new[]
                {
                new
                {
                    label = "Bookings",
                    data = dataSet,
                    backgroundColor = Chart.BackgroundColor,
                    borderColor = Chart.BorderColor,
                    borderWidth = 1
                }
            }
            };

            return Json(data);
        }


        public IActionResult Setting()
        {
            ViewBag.ActivePage = "Setting";
            return View();
        }

        public JsonResult GetEvents() {
            var userId = int.Parse(User.FindFirstValue("UserId"));

            var events = _context.Bookings.Where(booking => booking.UserId == userId)
                    .Join(_context.Rooms,
                               booking => booking.RoomId,
                               room => room.RoomId,
                               (booking, room) => new CalendarEvent
                               {
                                   Title = booking.MeetingTitle + " - " + room.RoomName + " (" + room.RoomLocation + ")",
                                   Start = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   End = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   Description = room.RoomLocation
                               })   
                         .ToList();

            return new JsonResult(events);
        }


        public IActionResult Bookings() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SearchBooking(string filter) {
           
            var bookings = (from booking in _context.Bookings
                            join room in _context.Rooms on booking.RoomId equals room.RoomId
                            join user in _context.Users on booking.UserId equals user.UserId
                            where string.IsNullOrEmpty(filter) || booking.MeetingTitle.ToLower().Contains(filter) ||
                            room.RoomName.ToLower().Contains(filter)
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

            return View("AdminDashboard", bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
            {
                return BadRequest("Invalid booking ID.");
            }

            bool success = await _bookingManager.CancelBooking(bookingId);

            if (!success)
            {
                return NotFound("Booking not found or already canceled.");
            }

            TempData["SuccessMessage"] = "Booking canceled successfully.";
            return RedirectToAction("Index");
        }


    }
}
