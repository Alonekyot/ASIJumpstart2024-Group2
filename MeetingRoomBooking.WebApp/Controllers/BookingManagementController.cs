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
using MeetingRoomBooking.Data.Models;

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
		[HttpGet]
		public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
			var rooms = _context.Rooms.ToList(); 	
			return View(rooms);
        }

        public IActionResult Create(int? roomId,string? roomName) {
            ViewBag.ActivePage = "BookingManagement";
            ViewBag.RoomId = roomId;
            ViewBag.RoomName = roomName;
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

            var events = _context.Bookings.Where(b => b.RoomId == roomId)
                    .Join(_context.Rooms,
                               booking => booking.RoomId,
                               room => room.RoomId,
                               (booking, room) => new CalendarEvent
                               {
                                   Title = booking.MeetingTitle ,
                                   Start = booking.MeetingDate.ToDateTime(booking.StartTime),
                                   End = booking.MeetingDate.ToDateTime(booking.EndTime),
                                   Description = room.RoomLocation + " - " + room.RoomName
                               })

                         .ToList();
            return new JsonResult(events);
        }
		[HttpPost]
		public ActionResult FilterRoom(RoomFilterViewModel filters)
		{
			// Retrieve all rooms from your database or repository
			var rooms = _context.Rooms.AsQueryable();

			if (!string.IsNullOrEmpty(filters.SearchText))
			{
				rooms = rooms.Where(r => r.RoomName.Contains(filters.SearchText) || r.RoomLocation.Contains(filters.SearchText));
			}
			if (!string.IsNullOrEmpty(filters.RoomCapacity))
			{
				var range = filters.RoomCapacity.Split(new char[] { '-' }); 
				if (range.Length == 2 && int.TryParse(range[0], out int minCapacity) && int.TryParse(range[1], out int maxCapacity))
				{
					rooms = rooms.Where(r => r.RoomCapacity >= minCapacity && r.RoomCapacity <= maxCapacity);
				}
			}

			if (filters.SelectedAmenities != null && filters.SelectedAmenities.Any())
			{
				
				if (filters.SelectedAmenities.Contains("Audio"))
				{
					rooms = rooms.Where(r => r.Audio);
				}
				if (filters.SelectedAmenities.Contains("Video"))
				{
					rooms = rooms.Where(r => r.Video);
				}
				if (filters.SelectedAmenities.Contains("Projector"))
				{
					rooms = rooms.Where(r => r.Projector);
				}
				if (filters.SelectedAmenities.Contains("WhiteBoard"))
				{
					rooms = rooms.Where(r => r.WhiteBoard);
				}
				if (filters.SelectedAmenities.Contains("LoudSpeaker"))
				{
					rooms = rooms.Where(r => r.Loudspeaker);
				}
			}

			return View("Index", rooms.ToList());
		}



	}
}
