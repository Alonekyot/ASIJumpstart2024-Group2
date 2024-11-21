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


        public IActionResult Index(int pageNumber = 1, int pageSize = 7) {
            ViewBag.ActivePage = "UserManagement";
            var users = _context.Users
                .Where(u => !u.Deleted)
                .OrderBy(u => u.FirstName) // Sorting logic, adjust as needed
                .Skip((pageNumber - 1) * pageSize) // Skip the previous pages' records
                .Take(pageSize) // Take only the current page's records
                .ToList();

            var userModel = new UserModel
            {
                Users = users, // Replace with your actual data-fetching logic
                CreateUser = new UserViewModel()
            };

            int totalRecords = _context.Users.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(userModel);
        }

        [HttpPost]
        public IActionResult SearchUser(string filter) {
            if (!string.IsNullOrEmpty(filter)) {
                var users = _context.Users
                    .Where(u => u.FirstName.ToLower().Contains(filter.ToLower()) || u.LastName.ToLower().Contains(filter.ToLower()))
                    .ToList();
                var userViewModels = users.Select(u => new UserViewModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();

                var model = new UserListViewModel
                {
                    dataList = userViewModels
                };
                return View("Index", users);
            }
            return RedirectToAction("Index");
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
        public async Task<IActionResult> CreateUser(UserModel model) {

            var newUser = model.CreateUser;

            if(_context.Users.Any(u => u.Email == newUser.Email)) {
                ModelState.AddModelError("Email", "Email is already in use.");
            }

            if (ModelState.IsValid) {
                var user = _userManager.Add(newUser);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors)) {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return RedirectToAction("Index");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int UserId) {
            var user = await _context.Users.FindAsync(UserId);
            if (user != null)
            {
               user.Deleted = true;
               await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        private bool UserExists(int Userid)
        {
            return _context.Users.Any(e => e.UserId == Userid);
        }
    }
}
