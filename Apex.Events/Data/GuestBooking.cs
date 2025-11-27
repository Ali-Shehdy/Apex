namespace Apex.Events.Data
{
    public class GuestBooking
    {
        public int GuestBookingId { get; set; } // Primary key
        public DateTime BookingDate { get; set; } // Date of booking
        public int GuestId { get; set; } // Foreign key to Guest
        public int EventId { get; set; } // Foreign key to Event

        // Navigation property to Guest
        public virtual Guest Guest { get; set; } = null!;

        // Navigation property to Event
        public virtual Event Event { get; set; } = null!;
    }
}
