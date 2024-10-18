using MeetingRoomBooking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.WebApp.Models;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    [RoleAuthorize(new int[] { 1, 2 })]// Only Roles with values 1 or 2, or Admin or Suer admin respectively
    public class UserManagementController : Controller {

        private readonly MeetingRoomBookingDbContext _context;
        private readonly UserManager _userManager;
        public UserManagementController(MeetingRoomBookingDbContext context,
                                        UserManager userManager) {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index(int pageNumber = 1, int pageSize = 5) {
            var users = _context.Users
                .Where(u => !u.Deleted)
                .OrderBy(u => u.FirstName) // Sorting logic, adjust as needed
                .Skip((pageNumber - 1) * pageSize) // Skip the previous pages' records
                .Take(pageSize) // Take only the current page's records
                .ToList();

            int totalRecords = _context.Users.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(users);
        }

        public IActionResult Create() {
            return View();
        }

        public void Detail(int? id) {
            var user = _context.Users
                .FirstOrDefault(u => u.UserId == id);
        }

        public IActionResult Details(int? id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserId == id);
            if(user != null) {
                return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel newUser) {

            if(_context.Users.Any(u => u.Email == newUser.Email)) {
                ModelState.AddModelError("Email", "Email is already in use.");
            }

            if (ModelState.IsValid) {
                var user = _userManager.Add(newUser);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(EditUserModel editedUser)
        {
            var user = await _context.Users.FindAsync(editedUser.UserId);
            ModelState.Remove(nameof(user.Password));
            if (user.Email != user.Email && _context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
            }
           
            
            if (ModelState.IsValid)
            {
                user = _userManager.Edit(editedUser, user);
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

    }
}
