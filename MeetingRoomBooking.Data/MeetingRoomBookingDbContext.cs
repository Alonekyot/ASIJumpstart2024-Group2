using Microsoft.EntityFrameworkCore;

using MeetingRoomBooking.Data.Models;


namespace MeetingRoomBooking.Data {
    public class MeetingRoomBookingDbContext : DbContext {

        public MeetingRoomBookingDbContext() { }
        public MeetingRoomBookingDbContext(DbContextOptions<MeetingRoomBookingDbContext> options) : base(options){ }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Default",
                    b => b.MigrationsAssembly("MeetingRoomBooking.WebApp"));
            }
        }
    }
}
