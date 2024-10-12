using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomBooking.WebApp.Controllers
{
    public class PunoController : Controller
    {
        public IActionResult AboutUs()
        {
            return View("~/Views/Puno2x/AboutUs.cshtml");
        }
        public IActionResult Contact()
        {
            return View("~/Views/Puno2x/ContactUs.cshtml");
        }
    }
}
