using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingRoomBooking.Services.Manager;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MeetingRoomBooking.Services.Interfaces;

namespace MeetingRoomBooking.Services.Managers {
    public class LoginManager: ILoginManager {

        private readonly MeetingRoomBookingDbContext _context;

        public LoginManager(MeetingRoomBookingDbContext meetingRoomBookingDbContext)
        {
            _context = meetingRoomBookingDbContext;
        }


        public async Task<(bool Success, string ErrorMessage, List<Claim> Claims)> LoginAsync(LoginViewModel model) {
            // Retrieve the user by email or phone, ensuring the account is not deleted
            var user = await _context.Users
                .Where(x => (x.Email == model.Email || x.Phone == model.Email) && !x.Deleted)
                .FirstOrDefaultAsync();

            if (user != null) {
                // Verify the password
                bool isPasswordValid = PasswordManager.VerifyPassword(model.Password, user.Password);

                if (isPasswordValid) {
                    // Create claims for authentication
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("Role", user.Role.ToString())
                    };
                    return (true, null, claims); // Login successful
                }
            }
            return (false, "Invalid email or password", null); // Generic error message for invalid login
        }

        public async Task<string> UpdatePasswordAsync(ForgotPasswordViewModel model) {
            // Check if new password and confirmation match
            if (model.NewPassword != model.ConfirmNewPassword) {
                return "Passwords do not match.";
            }

            // Find user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null) {
                return "User not found.";
            }

            // Encrypt and update password
            user.Password = PasswordManager.EncryptPassword(model.NewPassword);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return success message or empty string
            return string.Empty;
        }
    }
}
