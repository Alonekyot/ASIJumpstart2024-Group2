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

namespace MeetingRoomBooking.Services.Managers
{
	public class RoomManager : IRoomServices
	{
		private MeetingRoomBookingDbContext _context;

		public RoomManager(MeetingRoomBookingDbContext context) {
			_context = context;
		}


		public Room Add(CreateRoomModel model) {
			var room = new Room()
			{
				RoomName = model.RoomName,
				RoomLocation = model.RoomLocation,
				RoomCapacity = model.RoomCapacity,
				Audio = model.Audio,
				Video = model.Video,
				WhiteBoard = model.WhiteBoard,
				Projector = model.Projector,
				Loudspeaker = model.Loudspeaker,
				//Image = model.ImageFile,
				Available = true,
				Deleted = false
			};
			return room;
		}
	}
}
