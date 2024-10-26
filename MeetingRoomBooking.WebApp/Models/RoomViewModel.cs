using Microsoft.Extensions.Primitives;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.WebApp.Models
{
	public class RoomViewModel
	{
		[Key]
		public int RoomId { get; set; }
		public string RoomName{ get; set; }		

		public string Location { get; set; }

		public int SeatingCapacity { get; set; }

		public string Amenities { get; set; }

		public Byte[] RoomImage { get; set; }
	}
}
