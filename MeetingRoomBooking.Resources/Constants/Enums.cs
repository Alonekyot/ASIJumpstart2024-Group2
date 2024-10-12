namespace MeetingRoomBooking.Resources.Constants 
{
    public class Enums {

        public enum Status {
            Success,
            Error,
            CustomErr,
        }

        public enum LoginResult {
            Success = 0,
            Failed = 1,
        }

        public enum  Roles
        {
            User = 0,
            Admin = 1,
            SuperAdmin = 2,
        }

    }
}
