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

            // Create a booking object
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

            // Check for conflicts with recurring bookings
            if (newbook.IsRecurring && newbook.RecurringPattern != null && booking.RecurringEnd.HasValue) {
                var recurringInstances = GenerateRecurrence(booking);

                foreach (var instance in recurringInstances) {
                    if (IsMeetingConflict(instance)) {
                        return false; // Conflict detected; exit early
                    }
                }
            }
            // Check for conflicts with non-recurring booking
            else if (IsMeetingConflict(new BookingInstance
            {
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                MeetingDate = booking.MeetingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
            })) {
                return false; // Conflict detected
            }

            // Save the booking to the database
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync(); // Ensure BookingId is generated

            // Handle recurring bookings
            if (newbook.IsRecurring && newbook.RecurringPattern != null && booking.RecurringEnd.HasValue) {
                var recurringInstances = GenerateRecurrence(booking);
                await _context.BookingInstance.AddRangeAsync(recurringInstances);
            }

            await _context.SaveChangesAsync(); // Save BookingInstance records
            return true;
        }

        private List<BookingInstance> GenerateRecurrence(Booking booking) {
            var bookings = new List<BookingInstance>();
            DateOnly current = booking.MeetingDate;
            while(current <= booking.RecurringEnd) {
                bookings.Add(new BookingInstance
                {
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    BookingId = booking.BookingId,
                    MeetingTitle = booking.MeetingTitle,
                    MeetingDate = current,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
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
        private bool IsOverlap(List<BookingInstance> existingBookings, BookingInstance newBooking) {
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

        public bool ValidateRecurringBooking(List<BookingInstance> existingBookings, Booking recurring) {
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

        private bool IsMeetingConflict(BookingInstance book) {
            var existingBookings = _context.BookingInstance
                .Where(b => b.MeetingDate == book.MeetingDate && b.RoomId == book.RoomId)
                .ToList();

            foreach (var existingBooking in existingBookings) {
                // Check for time overlaps
                if ((book.StartTime >= existingBooking.StartTime && book.StartTime < existingBooking.EndTime) ||
                    (book.EndTime > existingBooking.StartTime && book.EndTime <= existingBooking.EndTime) ||
                    (book.StartTime <= existingBooking.StartTime && book.EndTime >= existingBooking.EndTime)) {
                    return true; // Conflict detected
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


    }
}
