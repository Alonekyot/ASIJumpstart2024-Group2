using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Services.Manager;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Services.Services {
    public class LoginService {

        private readonly MeetingRoomBookingDbContext _context;

        public LoginService(MeetingRoomBookingDbContext context) {
            _context = context;
        }

        public void ChangePassword(ForgotPasswordViewModel model) 
        {
            
        }

    }
}
