using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class BookingManagementController : Controller {
		private readonly MeetingRoomBookingDbContext _context;
		private readonly RoomManager _roomManager;
        private readonly BookingManager _bookingManager;
		public BookingManagementController(MeetingRoomBookingDbContext context,
										RoomManager roomManager,
                                        BookingManager bookingManager)
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

        [Route("Book/{roomName}")]
        public IActionResult Create(int? roomId) {

            ViewBag.ActivePage = "BookingManagement";
            ViewBag.RoomId = roomId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBooking newBook, int roomId) {
            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            if (ModelState.IsValid) {

                var success = _bookingManager.CreateBook(newBook, userId, roomId);
                return RedirectToAction("Index");
            }

            ViewBag.ActivePage = "BookingManagement";
            ViewBag.RoomId = roomId;
            return View("Create", newBook);
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
                                   End = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   Description = room.RoomLocation
                               })
                         .ToList();

            return new JsonResult(events);
        }
    }
}
