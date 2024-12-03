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
using NuGet.Protocol.Plugins;
using Microsoft.CodeAnalysis.Scripting;
using MeetingRoomBooking.Services.Manager;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class HomeController : Controller {

        private readonly IBookingManager _bookingManager;
        private readonly ILogger<HomeController> _logger;
        private readonly MeetingRoomBookingDbContext _context;
        private readonly ChartDataManager _chartDataManager;

        public HomeController(ILogger<HomeController> logger, MeetingRoomBookingDbContext context, IBookingManager bookingManager, ChartDataManager chartDataManager)
        {
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

            ViewBag.RoomCount = roomCount;
            ViewBag.TodaysBooking = todaysBooking;
            ViewBag.Recurrings = recurring;
            return View();
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

            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();
            var password = PasswordManager.DecryptPassword(user.Password);

            ViewBag.Password = password;

            return View();
        }

        public JsonResult GetEvents() {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            

            var events = _context.Bookings.Where(booking => booking.UserId == userId && booking.BookingStatus != "Canceled")
                    .Join(_context.Rooms,
                               booking => booking.RoomId,
                               room => room.RoomId,
                               (booking, room) => new CalendarEvent
                               {
                                   Title = booking.MeetingTitle + " - " + room.RoomName,
                                   Start = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   End = booking.MeetingDate.ToDateTime(booking.EndTime),
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


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userId = int.Parse(User.FindFirstValue("UserId"));
            var user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();
            var password = PasswordManager.DecryptPassword(user.Password);

            if (model.CurrentPassword != password)
            {
                ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                return View("Setting", model);
            }

            // Prevent reuse of the same password
            if (model.NewPassword == model.CurrentPassword)
            {
                ModelState.AddModelError("NewPassword", "The new password cannot be the same as the current password.");
                return View("Setting", model);
            }

            // Encrypt and save the new password
            user.Password = PasswordManager.EncryptPassword(model.NewPassword);
            _context.SaveChanges();

            // Set the success message for TempData
            TempData["SuccessMessage"] = "Your password has been successfully updated.";

            // Redirect to the "Setting" view to display the success message and reset the page state
            return RedirectToAction("Setting");
        }
    }
}
