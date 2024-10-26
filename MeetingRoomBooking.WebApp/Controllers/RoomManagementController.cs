using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers
{
    public class RoomManagementController : Controller
    {
        public IActionResult Index()
        {
			ViewBag.ActivePage = "RoomManagement";
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
