using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels
{
    public class RoomFilterModel
    {
        public string SearchText { get; set; }
        public string RoomCapacity { get; set; }
        public List<string> SelectedAmenities { get; set; }

        public RoomFilterModel()
        {
            SelectedAmenities = new List<string>();
        }
    }

}
