using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeetingRoomBooking.WebApp.Models {
    public class LoginViewModel {

        [Required(ErrorMessage = "Email or Phone number is required")]
        //[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format. Example: user@email.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
