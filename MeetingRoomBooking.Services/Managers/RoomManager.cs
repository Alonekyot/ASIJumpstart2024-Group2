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
	public class RoomManager
	{
		private MeetingRoomBookingDbContext _context;

		public RoomManager(MeetingRoomBookingDbContext context) {
			_context = context;
		}


        public async Task<bool> AddAsync(CreateRoomModel model) {
            try {
                byte[]? imageData = null;

                if (model.ImageFile != null) {
                    // Validate file type and size
                    if (!IsImageFile(model.ImageFile)) {
                        return false; // Invalid file type
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
                    Available = true,
                };
                _context.Rooms.Add(room);

                return true;
            }
            catch (Exception ex) {
                Console.Error.WriteLine($"Error while adding room: {ex.Message}");
                return false; 
            }
        }

        private bool IsImageFile(IFormFile file) {
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && permittedExtensions.Contains(extension);
        }

        public async Task<bool> Delete(int RoomId) {
            var room = await _context.Rooms.FindAsync(RoomId);
            if (room == null) {
                return false;
            }
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync(); 
            return true;
        }
    }
}
