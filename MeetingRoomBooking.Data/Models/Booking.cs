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
		public DateTime StartingDate { get; set; }
		public DateTime EndingDate { get; set; }
		public DateTime BookingDate { get; set; }
		public string BookingStatus { get; set; }
		public bool Deleted { get; set; }
		public bool Recurring { get; set; }

	}
}
