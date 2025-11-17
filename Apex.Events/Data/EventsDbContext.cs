using Microsoft.EntityFrameworkCore;
//using Apex.Venues.Data;

namespace Apex.Events.Data
{
    public class EventsDbContext : DbContext
    {
        //public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options)
        //{
        //}

        public DbSet<Events> Events { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<GuestBooking> GuestBookings { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Staffing> Staffings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship

            // 1 to Many R between Events & GuestBooking
            modelBuilder.Entity<GuestBooking>()
                .HasOne(gb => gb.Event)
                .WithMany(g => g.GuestBookings)
                .HasForeignKey(gb => gb.GuestId);

            modelBuilder.Entity<GuestBooking>()
                .HasOne(gb => gb.Event)
                .WithMany(e => e.GuestBookings)
                .HasForeignKey(gb => gb.EventId);

            // 1 to Many R between Events & Staffings
            modelBuilder.Entity<Staffing>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Staffings)
                .HasForeignKey(s => s.EventId);

            modelBuilder.Entity<Staffing>()
                .HasOne(s => s.Staff)
                .WithMany(st => st.Staffings)
                .HasForeignKey(s => s.StaffId);

            //// 1 to 0..1 R between Events & FoodBooking 
            //modelBuilder.Entity<Events>()
            //    .HasOne(e => e.FoodBooking)
            //    .WithOne()
            //    .HasForeignKey<Events>(e => e.FoodBookingId);

            //// 1 to 0..1 R between Events & Reservation
            //modelBuilder.Entity<Events>()
            //    .HasOne<Reservation>()
            //    .WithOne()
            //    .HasForeignKey<Events>(e => e.ReservationReference);

            // Seed data
            modelBuilder.Entity<Events>().HasData(
     new Events { EventId = 1, EventName = "Sample Event 1", EventDate = new DateTime(2024, 1, 1) },
     new Events { EventId = 2, EventName = "Sample Event 2", EventDate = new DateTime(2024, 1, 2) }
 );

            modelBuilder.Entity<Guest>().HasData(
                new Guest { GuestId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new Guest { GuestId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
            );


        }




        private string DbPath { get; set; } = string.Empty;

        // Constructor to set-up the database path and name
        public EventsDbContext()
        {
            var folder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "events.db");
        }

        // OnConfiguring to specify that the SQLite database engine will be used
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

  
    }
}
