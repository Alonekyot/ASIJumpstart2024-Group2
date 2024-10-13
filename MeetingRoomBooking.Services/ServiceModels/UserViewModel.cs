using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.ServiceModels {
    public class UserViewModel {

        [Required(ErrorMessage = "Firstname is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Remarks { get; set; } = "User";


    }

    public class UserListViewModel {
        [DisplayName("ID")]
        public string IdFilter { get; set; }

        //[Display(Name = "FirstName", ResourceType = typeof(Resources.Views.Screen))]
        public string FirstNameFilter { get; set; }

        public IEnumerable<UserViewModel> dataList { get; set; }
    }
}
