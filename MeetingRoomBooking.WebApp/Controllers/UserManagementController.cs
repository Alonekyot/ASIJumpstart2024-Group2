using MeetingRoomBooking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.WebApp.Models;
using MeetingRoomBooking.Services.ServiceModels;

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


        public IActionResult Index(int pageNumber = 1, int pageSize = 7) {
            ViewBag.ActivePage = "UserManagement";
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

        [HttpPost]
        public IActionResult SearchUser(string filter) {
            var users = _context.Users
                .Where(u => u.FirstName.ToLower().Contains(filter.ToLower()) || u.LastName.ToLower().Contains(filter.ToLower()))
                .ToList();
            return RedirectToAction("Index", users);
        }

        public IActionResult Create() {
            ViewBag.ActivePage = "UserManagement";
            return View();
        }

        public IActionResult Details(int? id)
        {
            ViewBag.ActivePage = "UserManagement";
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

    }
}
