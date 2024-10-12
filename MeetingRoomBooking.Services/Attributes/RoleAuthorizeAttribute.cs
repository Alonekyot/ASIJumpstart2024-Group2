using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MeetingRoomBooking.Services.Managers {


    //Gives authorized pages not accessible to anyone
    public class RoleAuthorizeAttribute : TypeFilterAttribute {
        public RoleAuthorizeAttribute(int[] allowedRoles) : base(typeof(RoleAuthorizeFilter)) {
            Arguments = new object[] { allowedRoles };
        }

        private class RoleAuthorizeFilter : IAuthorizationFilter {
            private readonly int[] _allowedRoles;
            public RoleAuthorizeFilter(int[] allowedRoles) {
                _allowedRoles = allowedRoles;
            }

            public void OnAuthorization(AuthorizationFilterContext context) {
                var user = context.HttpContext.User;
                var roleClaim = user.FindFirst("Role")?.Value;

                if (roleClaim == null || !_allowedRoles.Contains(int.Parse(roleClaim))) {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
