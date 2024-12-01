using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class BookingManagementController : Controller {
		private readonly MeetingRoomBookingDbContext _context;
		private readonly IRoomServices _roomManager;
        private readonly IBookingManager _bookingManager;
		public BookingManagementController(MeetingRoomBookingDbContext context,
										IRoomServices roomManager,
                                        IBookingManager bookingManager)
		{
			_context = context;
			_roomManager = roomManager;
            _bookingManager = bookingManager;

		}
		public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
			var rooms = _context.Rooms.ToList();
			return View(rooms);
        }

        public IActionResult Create(int? roomId) {
            ViewBag.ActivePage = "BookingManagement";
            ViewBag.RoomId = roomId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBooking newBook, int roomId) {
            if (string.IsNullOrWhiteSpace(newBook.MeetingTitle)) {
                ModelState.AddModelError("MeetingTitle", "Provide meeting title");
                TempData["Model"] = JsonConvert.SerializeObject(newBook);
                return RedirectToAction("Create", new { roomId });
            }

            DateOnly meetingDate = DateOnly.FromDateTime(newBook.MeetingDate);
            TimeOnly startTime = TimeOnly.FromDateTime(newBook.TimeStart);
            TimeOnly endTime = TimeOnly.FromDateTime(newBook.TimeEnd);
            DateTime meetingTimeStart = meetingDate.ToDateTime(startTime);
            DateTime meetingTimeEnd = meetingDate.ToDateTime(endTime);

            if (meetingTimeStart > meetingTimeEnd) {
                ModelState.AddModelError("TimeEnd", "A meeting can't end before it starts");
                TempData["Model"] = JsonConvert.SerializeObject(newBook);
                return RedirectToAction("Create", new { roomId });
            }
            else if (DateTime.Now > meetingTimeStart) {
                ModelState.AddModelError("TimeEnd", "The meeting is already starting or it has finished");
                TempData["Model"] = JsonConvert.SerializeObject(newBook);
                return RedirectToAction("Create", new { roomId });
            }

            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            if (ModelState.IsValid) {
                var success = await _bookingManager.CreateBook(newBook, userId, roomId);

                if (!success) {
                    ModelState.AddModelError("MeetingDate", "There might be some conflict with the bookings");
                    TempData["Model"] = JsonConvert.SerializeObject(newBook);
                    return RedirectToAction("Create", new { roomId });
                }

                return RedirectToAction("Index");
            }

            TempData["Model"] = JsonConvert.SerializeObject(newBook);
            return RedirectToAction("Create", new { roomId });
        }

        public JsonResult GetEvents() {
       
            var events = _context.Bookings
                    .Join(_context.Rooms,
                               booking => booking.RoomId,
                               room => room.RoomId,
                               (booking, room) => new CalendarEvent
                               {
                                   Title = booking.MeetingTitle + " - " + room.RoomName + "  (" + room.RoomLocation + ")",
                                   Start = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   End = booking.MeetingDate.ToDateTime(booking.EndTime),
                                   Description = room.RoomLocation
                               })
                         .ToList();

            return new JsonResult(events);
        }
    }
}
