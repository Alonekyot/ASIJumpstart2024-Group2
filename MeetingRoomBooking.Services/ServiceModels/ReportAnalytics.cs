using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {

    public class LeaderBoardLists {
        public IEnumerable<RoomLeaderBoard> RoomLeaders { get; set; }
        public IEnumerable<UserLeaderBoard> UserLeaders { get; set; }
    }

    public class RoomLeaderBoard {
        public string RoomName { get; set; }
        public int BookedTimes { get; set; }
    }
    public class UserLeaderBoard {
        public string Username { get; set; }
        public int BookedTimes { get; set; }
    }
}
