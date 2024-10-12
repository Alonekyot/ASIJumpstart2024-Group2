using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MeetingRoomBooking.Data;
using MeetingRoomBooking.Data.Models;
using MeetingRoomBooking.Services.Managers;
using System;
using System.Linq;
using MeetingRoomBooking.Services.Manager;

namespace MeetingRoomBooking.WebApp.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MeetingRoomBookingDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MeetingRoomBookingDbContext>>()))
            {
                // Look for any movies.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }
                context.Users.AddRange(
                    new User
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        Password = PasswordManager.EncryptPassword("1234"),
                        Email = "superadmin@gmail.com",
                        Phone = "0912345678",
                        Role = 2,
                        Remarks = "Super Admin",
                        Deleted = false
                    }              
                );
                context.SaveChanges();
            }
        }
    }
}
