	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	namespace MeetingRoomBooking.Data.Models
	{
		public class Booking
		{
			[Key]
			public int BookingId { get; set; }
			public int UserId { get; set; }

			[ForeignKey("UserId")]
			public User User { get; set; }
			public int RoomId { get; set; }

			[ForeignKey("RoomId")]
			public Room Room { get; set; }
			public string MeetingTitle { get; set; }
			public DateOnly MeetingDate { get; set; }
			public TimeOnly StartTime { get; set; }
			public TimeOnly EndTime { get; set; }
			public string BookingStatus { get; set; }
			public bool Recurring { get; set; }
			public string? RecurringPattern { get; set; }
			public DateOnly? RecurringEnd { get; set; }

		}
	}
