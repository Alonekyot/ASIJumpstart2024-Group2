using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.IO;
using System.Threading.Tasks;
using MeetingRoomBooking.Services.ServiceModels;
using MeetingRoomBooking.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MeetingRoomBooking.Services.Manager;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MeetingRoomBooking.Services.Managers;

namespace MeetingRoomBooking.WebApp.Controllers {
    public class AccountController : Controller {

        private readonly MeetingRoomBookingDbContext _context;
        private readonly LoginManager _loginManager;

        public AccountController(MeetingRoomBookingDbContext meetingRoomBookingDbContext, LoginManager loginManager)
        {
            _context = meetingRoomBookingDbContext;
            _loginManager = loginManager;
        }


        public IActionResult Index() {
            return View();
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (ModelState.IsValid) {
                var result = await _loginManager.LoginAsync(model);

                if (result.Success) {
                    // Login successful, creating cookie
                    var claimsIdentity = new ClaimsIdentity(result.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("Index", "Home");
                }

                // Add error message if login failed
                ModelState.AddModelError("Email", result.ErrorMessage);
            }
            return View(model);
        }


        public IActionResult Logout() {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPassword(ForgotPasswordViewModel model)
        {
            // Call the service method to update the password
            var errorMessage = await _loginManager.UpdatePasswordAsync(model);

            // If there's an error message, add it to ModelState and return the view
            if (!string.IsNullOrEmpty(errorMessage)) {
                if(errorMessage == "Passwords do not match.") {
                    ModelState.AddModelError("ConfirmNewPassword", errorMessage);
                }
                else {
                    ModelState.AddModelError("Email", errorMessage);
                }
                
                return View("ForgotPassword", model);
            }

            // Redirect to the login page on success
            return RedirectToAction("Login");

        }
    }
}
