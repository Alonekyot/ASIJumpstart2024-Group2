using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Claims;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;

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
                                 MeetingTitle = booking.MeetingTitle
                             }).ToList();

                
                return View("AdminDashboard", bookings); // Pass the bookings to the Admin view
            }
            else if (role == 0)
            {
                // For User: fetch bookings only for the logged-in user
                var bookings = (from booking in _context.Bookings
                                join room in _context.Rooms on booking.RoomId equals room.RoomId
                                where booking.UserId == userId
                                select new BookingModel
                                {                                  
                                    RoomName = room.RoomName,
                                    MeetingDate = booking.MeetingDate,
                                    StartTime = booking.StartTime,
                                    EndTime = booking.EndTime,
                                    RoomLocation = room.RoomLocation,
                                    BookingStatus = booking.BookingStatus,
                                    MeetingTitle = booking.MeetingTitle
                                }).ToList();

                return View("UserDashboard", bookings); // Pass the bookings to the User view
            }
            return View();

        }
        public IActionResult ReportAnalytics() {
            ViewBag.ActivePage = "Report & Analytics";

            int roomCount = _context.Rooms
                .Count();
            ViewBag.RoomCount = roomCount;
            return View();
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
                                   Title = booking.MeetingTitle + " - " + room.RoomName,
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
        
    }
}
