using MeetingRoomBooking.Data.Models;

namespace MeetingRoomBooking.WebApp.Models

{
    public class UserVewModel
    {
        public IEnumerable<User> Users { get; set; }
        public User SingleUser { get; set; }

    }
}
