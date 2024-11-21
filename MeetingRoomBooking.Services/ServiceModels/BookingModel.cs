using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class BookingModel {
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
            return $"MeetingTitle: {MeetingTitle}" +
                $"MeetingDate: {MeetingDate}";
        }

    }
}
