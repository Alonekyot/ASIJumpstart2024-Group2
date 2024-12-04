using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Data.Models {
    public class BookingInstance {
        [Key]
        public int BookingInstanceId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public string MeetingTitle { get; set; }
        public int BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }
        public DateOnly MeetingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
