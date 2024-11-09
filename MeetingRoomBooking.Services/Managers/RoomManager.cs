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

namespace MeetingRoomBooking.Services.Managers
{
	public class RoomManager : IRoomServices
	{
		public Room Add(CreateRoomModel model) {
			var room = new Room();
			room.RoomName = model.RoomName;
			room.RoomLocation = model.RoomLocation;
			room.RoomCapacity = model.RoomCapacity;
			room.Audio = model.Audio;
			room.Video = model.Video;
			room.WhiteBoard = model.WhiteBoard;
			room.Projector = model.Projector;
			room.Loudspeaker = model.Loudspeaker;
			room.Image = model.Image;
			room.Available = true;
			room.Deleted = false;

			return room;
		}

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

	}
}
