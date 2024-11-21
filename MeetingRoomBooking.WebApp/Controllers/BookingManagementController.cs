using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public JsonResult GetEvents(int roomId) {

            //var bookings = _bookingManager.GetBookings(roomId);

            //var events = bookings.Select(b => new
            //{
            //    title = b.MeetingTitle,
            //    start = b.MeetingDate.ToDateTime(b.StartTime),
            //    end = b.MeetingDate.ToDateTime(b.EndTime),
            //}).ToList();
            var events = new List<CalendarEvent>
            {
                new CalendarEvent { Title = "Event 1", Start = DateTime.Today, End = DateTime.Today.AddDays(1) },
                new CalendarEvent { Title = "Event 2", Start = new DateTime(2024, 11, 22, 8,0,0), End = new DateTime(2024, 11, 22, 12,0,0) },
            };
            return new JsonResult(events);
        }
    }
}
