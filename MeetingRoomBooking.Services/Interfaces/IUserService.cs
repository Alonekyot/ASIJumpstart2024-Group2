using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.ServiceModels;
using static MeetingRoomBooking.Resources.Constants.Enums;
using System.Collections.Generic;

namespace MeetingRoomBooking.Services.Interfaces {
    public interface IUserService {
        IEnumerable<UserViewModel> RetrieveAll(int? id = null, string firstName = null);
        UserViewModel RetrieveUser(int id);
        void Add(UserViewModel model);
        void Update(UserViewModel model);
        void Delete(int id);
        LoginResult AuthenticateUser(string userCode, string password, ref User user);
    }
}
