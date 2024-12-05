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

            // Handle recurring bookings
            if (newbook.IsRecurring && newbook.RecurringPattern != null && booking.RecurringEnd.HasValue) {
                // Generate recurring instances
                var recurringInstances = GenerateRecurrence(booking);

                // Check for conflicts
                foreach (var instance in recurringInstances) {
                    if (IsMeetingConflict(instance)) {
                        await _context.DisposeAsync();
                        return false; // Conflict detected, stop processing
                    }
                }

                // Add all valid instances to the database
                _context.Bookings.AddRange(recurringInstances);
            }
            else {
                // Handle non-recurring bookings
                if (IsMeetingConflict(booking)) {
                    await _context.DisposeAsync();
                    return false; // Conflict detected
                }

                _context.Bookings.Add(booking);
            }

            await _context.SaveChangesAsync();
            await _context.DisposeAsync();

            return true;
        }

        private List<Booking> GenerateRecurrence(Booking booking) {
            var bookings = new List<Booking>();
            DateOnly current = booking.MeetingDate;
            while(current <= booking.RecurringEnd) {
                bookings.Add(new Booking
                {
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    MeetingTitle = booking.MeetingTitle,
                    MeetingDate = current,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    BookingStatus = booking.BookingStatus,
                    Recurring = false, // Individual instances are not marked as recurring
                });
                current = booking.RecurringPattern switch
                {
                    "Daily" => current.AddDays(1),
                    "Weekly" => current.AddDays(7),
                    "Monthly" => current.AddMonths(1),
                    _ => throw new InvalidOperationException("Invalid Recurring Pattern")
                };
            }
            return bookings;
        }
        private bool IsOverlap(List<Booking> existingBookings, Booking newBooking) {
            foreach (var booking in existingBookings) {
                if (booking.RoomId == newBooking.RoomId &&
                    booking.MeetingDate == newBooking.MeetingDate &&
                    booking.StartTime < newBooking.EndTime &&
                    newBooking.StartTime < booking.EndTime) {
                    return true; // Overlap detected
                }
            }
            return false;
        }

        public bool ValidateRecurringBooking(List<Booking> existingBookings, Booking recurring) {
            var newBookings = GenerateRecurrence(recurring);

            foreach (var newBooking in newBookings) {
                if (IsOverlap(existingBookings, newBooking)) {
                    return false; // Conflict detected
                }
            }

            return true; // No conflicts
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

        private bool IsTimeOverlap(Booking book, Booking existingBooking) {
            return (book.StartTime >= existingBooking.StartTime && book.StartTime < existingBooking.EndTime) ||
                (book.EndTime > existingBooking.StartTime && book.EndTime <= existingBooking.EndTime) ||
                (book.StartTime <= existingBooking.StartTime && book.EndTime >= existingBooking.EndTime);

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
        public async Task<bool> DeleteBooking(int bookingId) {

            if (bookingId <= 0)
            {
                return false; // Invalid ID
            }
            var booking = await _context.Bookings.FirstOrDefaultAsync(r => r.BookingId == bookingId);
            if (booking == null)
            {
                return false; // Booking not found
            }
            booking.BookingStatus = "Canceled";
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;       
        }


    }
}
