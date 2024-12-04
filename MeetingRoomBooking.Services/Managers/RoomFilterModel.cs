using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.Managers
{
	public class RoomFilterViewModel
{
    public string SearchText { get; set; }
    public string RoomCapacity { get; set; }
    public List<string> SelectedAmenities { get; set; }

    public RoomFilterViewModel()
    {
        SelectedAmenities = new List<string>();
    }
}

}
