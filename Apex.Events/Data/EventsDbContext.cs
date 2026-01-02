using Microsoft.EntityFrameworkCore;

namespace Apex.Events.Data
{
    public class EventsDbContext : DbContext
    {
       
        public EventsDbContext(DbContextOptions<EventsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Guest> Guests { get; set; }
        public DbSet<GuestBooking> GuestBookings { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Staffing> Staffings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FIXED: Correct relationship GuestBooking → Event
            modelBuilder.Entity<GuestBooking>()
                .HasOne(gb => gb.Event)
                .WithMany(e => e.GuestBookings)
                .HasForeignKey(gb => gb.EventId);

            // FIXED: Relationship GuestBooking → Guest
            modelBuilder.Entity<GuestBooking>()
                .HasOne(gb => gb.Guest)
                .WithMany(g => g.GuestBookings)
                .HasForeignKey(gb => gb.GuestId);

            // Staff → Event
            modelBuilder.Entity<Staffing>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Staffings)
                .HasForeignKey(s => s.EventId);

            modelBuilder.Entity<Staffing>()
                .HasOne(s => s.Staff)
                .WithMany(st => st.Staffings)
                .HasForeignKey(s => s.StaffId);


            modelBuilder.Entity<Event>()
    .HasQueryFilter(e => !e.IsCancelled);

            // --- SEEDING ---
            //modelBuilder.Entity<Event>().HasData(
            //    new Event { EventId = 1, EventName = "Sample Event 1", EventDate = new DateTime(2024, 1, 1), EventType = EventType.Conference },
            //    new Event { EventId = 2, EventName = "Sample Event 2", EventDate = new DateTime(2024, 1, 2), EventType = EventType.Wedding }
            //);

            //modelBuilder.Entity<Guest>().HasData(
            //    new Guest { GuestId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            //    new Guest { GuestId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            //);
        }
    }
}
