using MeetingRoomBooking.Data.Models;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class RoomModel {
        public IEnumerable<Room>? Rooms { get; set; }
        public CreateRoomModel NewRoom { get; set; }

    }
    public class FilterModel { 
        public int Capacity { get; set; }
    }
}
