using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {
	public class CreateRoomModel {
		public string RoomName { get; set; }
		public string RoomLocation { get; set; }
		public int RoomCapacity { get; set; }
		public bool Audio { get; set; }
		public bool Video { get; set; }
		public bool WhiteBoard { get; set; }
		public bool Projector { get; set; }
		public bool Loudspeaker { get; set; }
		public byte[]? Image { get; set; }
	}
}
