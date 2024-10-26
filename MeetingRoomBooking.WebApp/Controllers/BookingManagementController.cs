using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers {
    public class BookingManagementController : Controller {
        public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
            return View();
        }
    }
}
