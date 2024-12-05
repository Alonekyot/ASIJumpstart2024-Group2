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
                MeetingStatus = booking.BookingStatus
            })) {
                return false; // Conflict detected
            }

            // Save the booking to the database
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync(); // Ensure BookingId is generated
            
            if (booking.Recurring) {
                var bookingInstances = GenerateRecurrence(booking);
                await _context.BookingInstance.AddRangeAsync(bookingInstances);
            }
            else {
                var bookingInstance = new BookingInstance()
                {
                    UserId = booking.UserId,
                    RoomId = booking.RoomId,
                    BookingId = booking.BookingId,
                    MeetingTitle = booking.MeetingTitle,
                    MeetingDate = booking.MeetingDate,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    MeetingStatus = booking.BookingStatus
                };
                await _context.BookingInstance.AddAsync(bookingInstance);
            }
            
            


            await _context.SaveChangesAsync(); // Save BookingInstance records
            return true;
        }

        public async Task<bool> EditBooking(EditBooking booking) {
            if (IsMeetingConflict(booking)) {
                return false;
            }
            var b = await _context.BookingInstance
                .FirstOrDefaultAsync(b => b.BookingInstanceId == booking.BookingId);
            if (b == null) {
                return false;
            }
            b.MeetingTitle = booking.MeetingTitle;
            b.MeetingDate = booking.MeetingDate;
            b.StartTime = booking.StartTime;
            b.EndTime = booking.EndTime;
            b.RoomId = booking.RoomId;

            await _context.SaveChangesAsync();
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
                    MeetingStatus = booking.BookingStatus
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

        public List<Booking> GetBookings(int roomId) {
            return _context.Bookings
                .Where(b => b.RoomId == roomId)
                .ToList();
        }

        private bool IsMeetingConflict(BookingInstance book) {
            var existingBookings = _context.BookingInstance
                .Where(b => b.MeetingDate == book.MeetingDate && b.RoomId == book.RoomId && b.MeetingStatus != "Canceled")
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
        private bool IsMeetingConflict(EditBooking book) {
            var existingBookings = _context.BookingInstance
                .Where(b => b.MeetingDate == book.MeetingDate && b.RoomId == book.RoomId && b.MeetingStatus != "Canceled" && book.BookingId != b.BookingInstanceId)
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

        public async Task<bool> CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
            {
                return false; // Invalid ID
            }

            var booking = await _context.BookingInstance.FirstOrDefaultAsync(r => r.BookingInstanceId == bookingId);

            if (booking == null)
            {
                return false; // Booking not found
            }

            // Update status to 'Canceled'
            booking.MeetingStatus = "Canceled";
            _context.BookingInstance.Update(booking);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
