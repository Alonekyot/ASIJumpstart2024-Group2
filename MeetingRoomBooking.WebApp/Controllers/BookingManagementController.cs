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

        public IActionResult Edit(int? id) {
            var b = _context.BookingInstance
                .FirstOrDefault(b => b.BookingInstanceId == id);
            var rooms = _context.Rooms.ToList();
            var meeting = new EditBooking()
            {
                BookingId = b.BookingInstanceId,
                MeetingTitle = b.MeetingTitle,
                MeetingDate = b.MeetingDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                RoomId = b.RoomId,
                Rooms = rooms
            };
            return View(meeting);
        }

        public IActionResult ViewAll() {

            int id = int.Parse(User.FindFirstValue("UserId"));

            var bookings = _context.Bookings
                .Where(o => o.BookingStatus != "Canceled" && o.UserId == id)
                .Include(b => b.BookingInstances.Where(k => k.MeetingStatus != "Canceled"))
                .ToList();
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(CreateBooking newBook, int roomId) {

            Console.WriteLine(newBook);
            if (string.IsNullOrWhiteSpace(newBook.MeetingTitle)) {
                ModelState.AddModelError("MeetingTitle", "Provide meeting title");
                ViewBag.RoomId = roomId;
                return View("Create");
            }

            DateOnly meetingDate = DateOnly.FromDateTime(newBook.MeetingDate);
            TimeOnly startTime = TimeOnly.FromDateTime(newBook.TimeStart);
            TimeOnly endTime = TimeOnly.FromDateTime(newBook.TimeEnd);
            DateTime meetingTimeStart = meetingDate.ToDateTime(startTime);
            DateTime meetingTimeEnd = meetingDate.ToDateTime(endTime);

            if (meetingTimeStart > meetingTimeEnd) {
                ModelState.AddModelError("TimeEnd", "A meeting can't end before it starts");
                ViewBag.RoomId = roomId;
                return View("Create");
            }
            else if (DateTime.Now > meetingTimeStart) {
                ModelState.AddModelError("TimeEnd", "The meeting is already starting or it has finished");
                ViewBag.RoomId = roomId;
                return View("Create");
            }

            int userId = int.Parse(User.FindFirst("UserId")?.Value);

            if (ModelState.IsValid) {
                var success = await _bookingManager.CreateBook(newBook, userId, roomId);

                if (!success) {
                    ModelState.AddModelError("MeetingDate", "There might be some conflict with the bookings");
                    TempData["Model"] = JsonConvert.SerializeObject(newBook);
                    ViewBag.RoomId = roomId;
                    return View("Create");
                }
				TempData["SuccessMessage"] = "Booked Successfully!";
				return RedirectToAction("Index");
            }
            else {
                foreach (var entry in ModelState) {
                    if (entry.Value.Errors.Count > 0) {
                        Console.WriteLine($"Key: {entry.Key}");
                        foreach (var error in entry.Value.Errors) {
                            Console.WriteLine($"Error: {error.ErrorMessage}");
                            if (error.Exception != null) {
                                Console.WriteLine($"Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }
            }

            ViewBag.RoomId = roomId;
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> CancelRecurring(int? id) {
            var booking = _context.Bookings
                .Include(a => a.BookingInstances)
                .FirstOrDefault(b => b.BookingId == id);
            if(booking != null) {
                booking.BookingStatus = "Canceled";
                foreach(var instance in booking.BookingInstances) {
                    instance.MeetingStatus = "Canceled";
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewAll");
        }
        [HttpPost]
        public async Task<IActionResult> CancelBook(int? meetingId) {
            var meeting = _context.BookingInstance
                .FirstOrDefault(m => m.BookingInstanceId == meetingId);
            meeting.MeetingStatus = "Canceled";
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewAll");
        }

        public JsonResult GetEvents() {
       
            var events = _context.BookingInstance
                    .Where(b => b.MeetingStatus != "Canceled")
                    .Join(_context.Rooms,
                               booking => booking.RoomId,
                               room => room.RoomId,
                               (booking, room) => new CalendarEvent
                               {
                                   Title = booking.MeetingTitle + " - " + room.RoomName + "  (" + room.RoomLocation + ")",
                                   Start = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   End = booking.MeetingDate.ToDateTime(booking.EndTime)
                               })
                         .ToList();

            return new JsonResult(events);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(EditBooking book) {
            if (!ModelState.IsValid) {
                Console.WriteLine("Hello");
                return RedirectToAction("Edit", new { id = book.BookingId });
            }

            var success = await _bookingManager.EditBooking(book);

            if (!success) {
                TempData["ErrorMessage"] = "Unable to edit booking due to conflicts.";
                return RedirectToAction("Edit", new { id = book.BookingId });
            }

            TempData["SuccessMessage"] = "Booking edited successfully!";
            return RedirectToAction("Index", "Home");
        }

    }
}
