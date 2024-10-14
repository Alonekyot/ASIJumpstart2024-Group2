using MeetingRoomBooking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.Manager;
using MeetingRoomBooking.WebApp.Models;
using MeetingRoomBooking.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    [RoleAuthorize(new int[] { 1, 2 })]// Only Roles with values 1 or 2, or Admin or Suer admin respectively
    public class UserManagementController : Controller {

        private readonly MeetingRoomBookingDbContext _context;
        public UserManagementController(MeetingRoomBookingDbContext context) {
            _context = context;
        }

        
        public IActionResult Index() {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Create() {
            return View();
        }

        public IActionResult Details(int? id)
        {
            foreach (var user in _context.Users)
            {
                if (user.UserId == id)
                {
                   System.Diagnostics.Debug.WriteLine("Hello!!!!!!!!");
                    return View(user);
                }
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName, FirstName, Email, Phone, Password")] User user) {

            if (_context.Users.Any(u => u.Email == user.Email)) {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
            user.Password = PasswordManager.EncryptPassword(user.Password);
            user.Role = 0; // Default to User role (0 = User)

            switch (user.Role) {
                case 0:
                    user.Remarks = "User";
                    break;
                case 1:
                    user.Remarks = "Admin";
                    break;
                case 2:
                    user.Remarks = "Super Admin";
                    break;
                default:
                    user.Remarks = "Unknown";
                    break;
            }

            user.Deleted = false; // Default to not deleted

            if (ModelState.IsValid) {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("Model is not valid");

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) {
                Console.WriteLine(error.ErrorMessage);
            }


            return View(user); // Return the view with validation errors
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("UserId, LastName, FirstName, Email, Phone")] User user)
        {
            var existingUser = await _context.Users.FindAsync(user.UserId);
            ModelState.Remove(nameof(user.Password));
            if (existingUser.Email != user.Email && _context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
           
            
            if (existingUser == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    existingUser.LastName = user.LastName;
                    existingUser.FirstName = user.FirstName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Reached the end");
                return RedirectToAction(nameof(Index));
            }
           
            return View(existingUser);







        }

        private bool UserExists(int Userid)
        {
            return _context.Users.Any(e => e.UserId == Userid);
        }
    }
}
