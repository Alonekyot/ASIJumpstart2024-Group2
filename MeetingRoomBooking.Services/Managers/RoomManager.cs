using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.Services.Manager;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingRoomBooking.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace MeetingRoomBooking.Services.Managers
{
	public class RoomManager : IRoomServices
	{
		private readonly MeetingRoomBookingDbContext _context;

		public RoomManager(MeetingRoomBookingDbContext context) {
			_context = context;
		}


        public async Task<bool> AddAsync(CreateRoomModel model) {
            try {
                byte[]? imageData = null;

                if (model.ImageFile != null) {
                    if (model.ImageFile.Length > 4 * 1024 * 1024) { // 4MB size limit
                        throw new InvalidOperationException("File size exceeds the allowed limit.");
                    }

                    using (var memoryStream = new MemoryStream()) {
                        await model.ImageFile.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                var room = new Room
                {
                    RoomName = model.RoomName,
                    RoomLocation = model.RoomLocation,
                    RoomCapacity = model.RoomCapacity,
                    Audio = model.Audio,
                    Video = model.Video,
                    WhiteBoard = model.WhiteBoard,
                    Projector = model.Projector,
                    Loudspeaker = model.Loudspeaker,
                    Image = imageData,
                    //Available = true,
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) {
                // Log full exception details for troubleshooting
                Console.Error.WriteLine($"Error while adding room: {ex}");
                return false;
            }
        }

        //private bool IsImageFile(IFormFile file) {
        //    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    return !string.IsNullOrEmpty(extension) && permittedExtensions.Contains(extension);
        //}

        public Room Edit(EditRoomModel model, Room room)
		{
			room.RoomName = model.RoomName;
			room.RoomLocation = model.RoomLocation;
			room.RoomCapacity = model.RoomCapacity;
			room.Audio = model.Audio;
			room.Video = model.Video;
			room.WhiteBoard = model.WhiteBoard;
			room.Projector = model.Projector;
			room.Loudspeaker = model.Loudspeaker;
			room.Image = model.Image;


			return room;
		}

        public async Task<bool> Delete(int roomId) {
            var room = _context.Rooms
                .FirstOrDefault(r => r.RoomId == roomId);
            if (room != null) {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

	}
}
