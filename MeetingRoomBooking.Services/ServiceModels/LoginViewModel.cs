using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class LoginViewModel {

        [Required(ErrorMessage = "Email or Phone number is required")]
        //[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format. Example: user@email.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class ForgotPasswordViewModel {

        [Required(ErrorMessage = "Email or Phone number is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmation password is required")]
        public string ConfirmNewPassword { get; set; }
    }
}
