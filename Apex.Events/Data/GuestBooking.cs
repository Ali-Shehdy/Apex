namespace Apex.Events.Data
{
    public class GuestBooking
    {
        public int GuestBookingId { get; set; } // Primary key
        public DateTime BookingDate { get; set; } // Date of booking
        public int GuestId { get; set; } // Foreign key to Guest



        // Navigation property
        public Guest Guest { get; set; } = null!; // Relation property to Guest
        public int EventId { get; set; } // Foreign key to Event
        public Event Event { get; set; } = null!; // Relation property to Event
    }
}
