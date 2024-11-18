using MeetingRoomBooking.Data;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
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
			var rooms = _context.Rooms.ToList()
				.Where(u => !u.Deleted);

			var roomModel = new RoomModel
			{
				Rooms = rooms,
				NewRoom = new CreateRoomModel()
			};

			return View(roomModel);
        }

		[HttpPost]
        public async Task<IActionResult> Create(RoomModel model) {
            if (!ModelState.IsValid) {
                return View("Index", model.NewRoom);
            }

            try {
                var success = await _roomManager.AddAsync(model.NewRoom);
                if (success) {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else {
                    ModelState.AddModelError("", "Unable to add the room. Please try again.");
                }
            }
            catch (Exception ex) {
                ModelState.AddModelError("", "An error occurred while creating the room. Please try again.");
            }

            return View("Index", model.NewRoom);
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
