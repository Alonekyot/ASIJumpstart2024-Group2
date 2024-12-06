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
		private readonly IRoomServices _roomManager;
		public RoomManagementController(MeetingRoomBookingDbContext context,
										IRoomServices roomManager)
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

        public IActionResult Edit(int id) {
            var room = _context.Rooms.FirstOrDefault(u => u.RoomId == id);
            if (room != null) {
                return View(room);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoomModel editedRoom) {
            var room = await _context.Rooms.FindAsync(editedRoom.RoomId);

			

			if (ModelState.IsValid) {
                room = await _roomManager.Edit(editedRoom, room);
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomModel model) {

            var newRoom = model.NewRoom;
            if (!ModelState.IsValid) {
                return RedirectToAction("Index");
             }

            byte[]? imageData = null;

                if (newRoom.ImageFile != null) {

                    using (var memoryStream = new MemoryStream()) {
                        await newRoom.ImageFile.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                var room = new Room
                {
                    RoomName = newRoom.RoomName,
                    RoomLocation = newRoom.RoomLocation,
                    RoomCapacity = newRoom.RoomCapacity,
                    Audio = newRoom.Audio,
                    Video = newRoom.Video,
                    WhiteBoard = newRoom.WhiteBoard,
                    Projector = newRoom.Projector,
                    Loudspeaker = newRoom.Loudspeaker,
                    Image = imageData,
                };

                _context.Rooms.Add(room);
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
