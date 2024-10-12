using MeetingRoomBooking.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.Interfaces {
    internal interface ICrudService {
        IEnumerable<UserViewModel> RetrieveAll(int? id = null, string firstName = null);
        UserViewModel RetrieveUser(int id);
        void Add(UserViewModel model);
        void Update(UserViewModel model);
        void Delete(int id);
    }
}
