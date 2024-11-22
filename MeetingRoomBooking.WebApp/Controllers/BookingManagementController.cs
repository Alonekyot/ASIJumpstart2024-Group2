using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
