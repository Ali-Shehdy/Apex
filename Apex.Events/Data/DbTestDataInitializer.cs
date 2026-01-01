namespace Apex.Events.Data
{
    public class DbTestDataInitializer
    {

        private readonly EventsDbContext _context;
        public DbTestDataInitializer(EventsDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {

            if (_context.Events.Any())
            {
                return; // DB has been seeded
            }
            var eventList= new List<Event>
            {
                new Event { EventId = 1, EventName = "Sample Event 1", EventDate = new DateTime(2024, 1, 1)},
                new Event { EventId = 2, EventName = "Sample Event 2", EventDate = new DateTime(2024, 1, 2) }
            };
            _context.Events.AddRange(eventList);
            _context.SaveChanges();

            var staffList = new List<Staff>
            {
                new Staff { StaffId = 1, FirstName = "John", LastName ="Doe", Role = "Coordinator" },
                new Staff { StaffId = 2, FirstName = "Jane", LastName ="Smith", Role = "Manager" }
            };

            _context.Staffs.AddRange(staffList);
            _context.SaveChanges();

            var guestList = new List<Guest>
            {
                new Guest { GuestId = 1, FirstName = "Alice", LastName = "Johnson", Email = "A.Johnson@gmail.com" },
                new Guest { GuestId = 2, FirstName = "Bob", LastName = "Brown", Email = "B.Brown" }
            };

            _context.Guests.AddRange(guestList);
            _context.SaveChanges();

            var guestBookingList = new List<GuestBooking>
            {
                new GuestBooking { GuestBookingId = 1, GuestId = 1, EventId = 1 },
                new GuestBooking { GuestBookingId = 2, GuestId = 2, EventId = 2 }
            };
            _context.GuestBookings.AddRange(guestBookingList);
            _context.SaveChanges();


            var staffingList = new List<Staffing>
            {
                new Staffing { StaffingId = 1, StaffId = 1, EventId = 1 },
                new Staffing { StaffingId = 2, StaffId = 2, EventId = 2 }
            };
            _context.Staffings.AddRange(staffingList);
            _context.SaveChanges();
        }
    }
}
