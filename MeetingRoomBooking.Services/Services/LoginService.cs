using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Manager;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Services.Services {
    public class LoginService {

        private readonly MeetingRoomBookingDbContext _context;

        public LoginService(MeetingRoomBookingDbContext context) {
            _context = context;
        }

        public async Task<(bool isSuccess, string errorMessage)> ChangePassword(ForgotPasswordViewModel model) 
        {
            // Check if the new password and confirmation match
            if (model.NewPassword != model.ConfirmNewPassword) {
                return (false, "The new password and confirmation do not match.");
            }

            // Find the user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) {
                return (false, "No user found with that email address.");
            }

            // Encrypt the new password and update the user's password
            user.Password = PasswordManager.EncryptPassword(model.NewPassword);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return (true, null); // Password changed successfully
        }

    }
}
