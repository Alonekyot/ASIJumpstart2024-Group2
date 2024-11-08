using MeetingRoomBooking.Services.Managers;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers
{
    [Authorize]
    [RoleAuthorize(new int[] { 1, 2 })]
    public class RoomManagementController : Controller
    {
        public IActionResult Index()
        {
			ViewBag.ActivePage = "RoomManagement";
			return View();
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomModel model) {
            if(ModelState.IsValid) {
                return View("Index");
            }
			return View("Index", model);
		}
    }
}
