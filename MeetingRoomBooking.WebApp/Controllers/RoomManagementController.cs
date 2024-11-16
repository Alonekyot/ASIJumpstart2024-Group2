using MeetingRoomBooking.Data;
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
			var rooms = _context.Rooms.ToList()
				.Where(u => !u.Deleted);
			return View(rooms);
        }

        public IActionResult Create() {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(CreateRoomModel newRoom) {
            if(ModelState.IsValid) {
				var room  = _roomManager.Add(newRoom);
				_context.Add(room);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View("Index", newRoom);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int RoomId)
		{
			var room = await _context.Rooms.FindAsync(RoomId);
			if (room != null)
			{
				room.Deleted = true;
				await _context.SaveChangesAsync();
			}

			return RedirectToAction("Index");
		}

        public IActionResult Edit(int id)
        {
			var room = _context.Rooms.FirstOrDefault(u => u.RoomId == id);
			if (room != null)
			{
				return View(room);
			}
			return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditRoomModel editedRoom)
		{
			var room = await _context.Rooms.FindAsync(editedRoom.RoomId);
			Console.WriteLine(editedRoom.RoomId+"---------------------------------------");
			
			if (ModelState.IsValid)
			{
				room = _roomManager.Edit(editedRoom, room);
				_context.Update(room);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(room);
		}
	}
}
