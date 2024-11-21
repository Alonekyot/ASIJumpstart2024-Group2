using MeetingRoomBooking.Data;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using MeetingRoomBooking.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.WebApp.Controllers
{

	public class RoomManagementController : Controller
	{
		private readonly MeetingRoomBookingDbContext _context;
		private readonly RoomManager _roomManager;
		public RoomManagementController(MeetingRoomBookingDbContext context,
										RoomManager roomManager)
		{
			_context = context;
			_roomManager = roomManager;

		}

		public IActionResult Index()
        {
			ViewBag.ActivePage = "RoomManagement";
			var rooms = _context.Rooms.ToList();

			var roomModel = new RoomModel
			{
				Rooms = rooms,
				NewRoom = new CreateRoomModel()
			};

			return View(roomModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomModel model) {

            var newRoom = model.NewRoom;
            if (!ModelState.IsValid) {
                return RedirectToAction("Index");
            }

            var success = _roomManager.AddAsync(newRoom);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int RoomId) {
            bool success = await _roomManager.Delete(RoomId);
            if (!success) {
                return NotFound("Room not found.");
            }
            return RedirectToAction("Index");
        }

    }
}
