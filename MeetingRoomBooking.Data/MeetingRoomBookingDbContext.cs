using Microsoft.EntityFrameworkCore;

using MeetingRoomBooking.Data.Models;


namespace MeetingRoomBooking.Data {
    public class MeetingRoomBookingDbContext : DbContext {

        public MeetingRoomBookingDbContext() { }
        public MeetingRoomBookingDbContext(DbContextOptions<MeetingRoomBookingDbContext> options) : base(options){ }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingInstance> BookingInstance { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
			// Configure the foreign keys using Fluent API
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.User)           // Booking has one User
				.WithMany(u => u.booking)      // User can have many Bookings
				.HasForeignKey(b => b.UserId); // The foreign key in Booking is UserId

			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Room)           // Booking has one Room
				.WithMany(r => r.booking)     // Room can have many Bookings
				.HasForeignKey(b => b.RoomId); // The foreign key in Booking is RoomId

            modelBuilder.Entity<BookingInstance>()
                .HasOne(b => b.Booking)
                .WithMany(u => u.BookingInstances)
                .HasForeignKey(b => b.BookingId);
		}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Default",
                    b => b.MigrationsAssembly("MeetingRoomBooking.WebApp"));
            }
        }
    }
}
