using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers {

    [Authorize]
    public class BookingManagementController : Controller {
        public IActionResult Index() {
            ViewBag.ActivePage = "BookingManagement";
            return View();
        }
    }
}
