using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRoomBooking.Services.Interfaces
{
    public interface IBookingManager
    {
        public Task<bool> CreateBook(CreateBooking newbook, int userId, int roomId);
        public List<Booking> GetBookings(int roomId);
        public Task<bool> CancelBooking(int bookingId);
        public Task<bool> DeleteBooking(int bookingId);
    }
}
