using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Data.Models
{
	public class Room
	{		
		[Key]
		public int RoomId { get; set; }
		public string RoomName { get; set; }

		public string Location { get; set; }

		public int SeatingCapacity { get; set; }

		public string Amenities { get; set; }

		public Byte[] RoomImage { get; set; }
	
	}
}
