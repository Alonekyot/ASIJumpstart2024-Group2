using MeetingRoomBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class BookingModel {
        public string UserName { get; set; }
        public string RoomName { get; set; }
        public string RoomLocation { get; set; }
        public DateOnly MeetingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string BookingStatus { get; set; }
        public string MeetingTitle { get; set; }
        public int BookingID { get; set; }
    }
    
    public class CreateBooking {
        public string MeetingTitle { get; set; }
        public DateTime MeetingDate { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurringPattern { get; set; }
        public DateTime? RecurringEnd { get; set; }

        public override string ToString() {
            return $"MeetingTitle: {MeetingTitle}, " +
                   $"MeetingDate: {MeetingDate}, " +
                   $"TimeStart: {TimeStart}, " +
                   $"TimeEnd: {TimeEnd}, " +
                   $"IsRecurring: {IsRecurring}, " +
                   $"RecurringPattern: {RecurringPattern ?? "N/A"}, " +
                   $"RecurringEnd: {(RecurringEnd.HasValue ? RecurringEnd.Value.ToString() : "N/A")}";
        }

    }
}
