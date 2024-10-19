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
    }
}
