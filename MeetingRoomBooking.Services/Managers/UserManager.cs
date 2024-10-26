using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingRoomBooking.Services.Manager;

namespace MeetingRoomBooking.Services.Managers
{
    public class UserManager : IUserService {
        public User Add(UserViewModel model) {
            var user = new User();

            user.LastName = model.LastName;
            user.FirstName = model.FirstName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.Remarks = model.Remarks;
            user.Deleted = false;
            switch (model.Remarks) {
                case "User":
                    user.Role = 0;
                    break;
                case "Admin":
                    user.Role = 1;
                    break;
                case "Super admin":
                    user.Role = 2;
                    break;
                default: user.Role = 0; break;
            }


            return user;
        }

        public User Edit(EditUserModel editedUser, User user)
        {
            user.LastName = editedUser.LastName;
            user.FirstName = editedUser.FirstName;
            user.Email = editedUser.Email;
            user.Phone = editedUser.Phone;
            

            return user;
        }

        
    }
}
