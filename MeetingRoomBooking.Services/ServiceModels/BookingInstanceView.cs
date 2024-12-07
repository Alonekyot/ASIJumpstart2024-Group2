using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels
{
    public class BookingInstanceView
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public DateOnly MeetinDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
