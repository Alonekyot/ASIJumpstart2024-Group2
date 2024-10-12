using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBooking.Data.Models {
    [Index(nameof(Email), IsUnique = true)]
    public class User {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Role { get; set; }
        public string? Remarks { get; set; }
        public bool Deleted { get; set; }
    }
}
