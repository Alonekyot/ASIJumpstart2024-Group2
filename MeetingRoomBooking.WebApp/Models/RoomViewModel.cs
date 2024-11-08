using Microsoft.Extensions.Primitives;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.WebApp.Models
{
	public class RoomViewModel
	{
		[Key]
		public int RoomId { get; set; }
		public string RoomName { get; set; }
		public string RoomLocation { get; set; }
		public int RoomCapacity { get; set; }
		public bool Audio { get; set; }
		public bool Video { get; set; }
		public bool WhiteBoard { get; set; }
		public bool Projector { get; set; }
		public bool Loudspeaker { get; set; }
		public byte[]? Image { get; set; }		
		public bool Available {  get; set; }
		public bool Deleted { get; set; }
	}
}
