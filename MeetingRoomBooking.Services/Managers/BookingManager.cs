using MeetingRoomBooking.Data;
/*using MeetingRoomBooking.Data.Migrations;*/
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Interfaces;
using MeetingRoomBooking.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MeetingRoomBooking.Services.Managers {
    public class BookingManager: IBookingManager{
        private readonly MeetingRoomBookingDbContext _context;

        public BookingManager(MeetingRoomBookingDbContext context) {
            _context = context;
        }

        public async Task<bool> CreateBook(CreateBooking newbook, int userId, int roomId) {
            if (newbook == null) {
                return false;
            }
            var booking = new Booking()
            {
                UserId = userId,
                RoomId = roomId,
                MeetingTitle = newbook.MeetingTitle,
                MeetingDate = DateOnly.FromDateTime(newbook.MeetingDate),
                StartTime = TimeOnly.FromDateTime(newbook.TimeStart),
                EndTime = TimeOnly.FromDateTime(newbook.TimeEnd),
                BookingStatus = MeetingRoomBooking.Resources.Constants.Enums.BookStatus.Scheduled.ToString(),
                Recurring = newbook.IsRecurring,
                RecurringPattern = newbook.RecurringPattern,
                RecurringEnd = newbook.RecurringEnd.HasValue ? DateOnly.FromDateTime(newbook.RecurringEnd.Value) : (DateOnly?)null
            };

            bool isConflict = IsMeetingConflict(booking);
            if (isConflict) {
                await _context.DisposeAsync();
                return false;
            }
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            await _context.DisposeAsync();

            return true;
        }

        
        public List<Booking> GetBookings(int roomId) {
            return _context.Bookings
                .Where(b => b.RoomId == roomId)
                .ToList();
        }

        public bool IsMeetingConflict(Booking book) {
            var existingBookings = _context.Bookings
                .Where(b => b.MeetingDate == book.MeetingDate)
                .Where(o => o.RoomId == book.RoomId)
                .ToList();

            foreach (var existingBooking in existingBookings) {
                // Check if there is a time overlap
                if ((book.StartTime >= existingBooking.StartTime && book.StartTime < existingBooking.EndTime) ||
                    (book.EndTime > existingBooking.StartTime && book.EndTime <= existingBooking.EndTime) ||
                    (book.StartTime <= existingBooking.StartTime && book.EndTime >= existingBooking.EndTime)) {
                    return true; // Conflict found
                }
            }
            return false; // No conflict
        }

        public async Task<bool> CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
            {
                return false; // Invalid ID
            }

            var booking = await _context.Bookings.FirstOrDefaultAsync(r => r.BookingId == bookingId);

            if (booking == null)
            {
                return false; // Booking not found
            }

            // Update status to 'Canceled'
            booking.BookingStatus = "Canceled";
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
