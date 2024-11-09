using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Managers;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers {
    public class BookingManagementController : Controller {
		private readonly MeetingRoomBookingDbContext _context;
		private readonly RoomManager _roomManager;
		public BookingManagementController(MeetingRoomBookingDbContext context,
										RoomManager roomManager)
		{
			_context = context;
			_roomManager = roomManager;

		}
		public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
			var rooms = _context.Rooms.ToList()
				.Where(u => !u.Deleted);
			return View(rooms);
        }
    }
}
