using MeetingRoomBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class EditBooking {
        public int BookingId { get; set; }
        public string MeetingTitle { get; set; }
        public DateOnly MeetingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int RoomId { get; set; }
        public List<Room>? Rooms { get; set; }
    }
}
