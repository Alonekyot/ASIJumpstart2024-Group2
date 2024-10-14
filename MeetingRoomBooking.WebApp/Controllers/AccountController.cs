using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.IO;
using System.Threading.Tasks;
using MeetingRoomBooking.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MeetingRoomBooking.Services.Manager;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBooking.Services.ServiceModels;

namespace MeetingRoomBooking.WebApp.Controllers {
    public class AccountController : Controller {

        private readonly MeetingRoomBookingDbContext _context;

        public AccountController(MeetingRoomBookingDbContext meetingRoomBookingDbContext) {
            _context = meetingRoomBookingDbContext;
        }


        public IActionResult Index() {
            return View(_context.Users.ToList());
        }

        public IActionResult Login() {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                // Retrieve the user by email or phone, ensuring the account is not deleted
                var user = _context.Users
                    .Where(x => (x.Email == model.Email || x.Phone == model.Email) && !x.Deleted)
                    .FirstOrDefault();

                if (user != null) {
                    // Decrypt the stored password
                    string decryptedPassword = PasswordManager.DecryptPassword(user.Password);

                    // Compare the decrypted password with the entered password
                    if (decryptedPassword == model.Password) {
                        // Login successful, creating cookie
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                            new Claim("UserId", user.UserId.ToString()),
                            new Claim("Role", user.Role.ToString())
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        ModelState.AddModelError("", "Email or Password is not found");
                    }
                }
                else {
                    ModelState.AddModelError("", "Email or Password is not found");
                }
            }
            return View(model);
        }


        public IActionResult Logout() {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPassword(ForgotPasswordViewModel newPasswordModel) {
            // Check if the new password and confirmation match
            if (newPasswordModel.NewPassword != newPasswordModel.ConfirmNewPassword) {
                ModelState.AddModelError("ConfirmNewPassword", "The new password and confirmation do not match.");
            }

            if (ModelState.IsValid) {
                // Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == newPasswordModel.Email);
                if (user == null) {
                    ModelState.AddModelError("Email", "No user found with that email address.");
                    return View("ForgotPassword", newPasswordModel);
                }

                // Encrypt the new password and update the user's password
                user.Password = PasswordManager.EncryptPassword(newPasswordModel.NewPassword);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the login page after successfully changing the password
                return RedirectToAction("Login");
            }

            // If validation fails, return the view with the validation errors
            return View("ForgotPassword", newPasswordModel);

        }
    }
}
