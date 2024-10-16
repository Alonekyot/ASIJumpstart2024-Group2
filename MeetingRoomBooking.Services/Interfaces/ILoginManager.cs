using MeetingRoomBooking.Services.ServiceModels;
using System.Security.Claims;

namespace MeetingRoomBooking.Services.Interfaces {
    public interface ILoginManager {
        Task<(bool Success, string ErrorMessage, List<Claim> Claims)> LoginAsync(LoginViewModel model);
        Task<string> UpdatePasswordAsync(ForgotPasswordViewModel model);
    }
}
