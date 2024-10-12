using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MeetingRoomBooking.WebApp.Controllers {


    public class BaseController : Controller {

        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            var fullName = User.FindFirstValue(ClaimTypes.Name);
            var userId = User.FindFirstValue("UserId");
            var role = User.FindFirstValue("Role");

            ViewData["FullName"] = fullName;
            ViewData["UserId"] = userId;
            ViewData["Role"] = role;
        }
    }
}
