using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.Interfaces
{
	public interface IRoomServices	{
		public Task<bool> AddAsync(CreateRoomModel model);
        public Task<Room> Edit(EditRoomModel model, Room room);
        public Task<bool> Delete(int roomId);

    }
}
