//using Apex.Catering.Data;
//using Apex.Venues.Data;


namespace Apex.Events.Data
{
    public class Events
    {
        public int EventId { get; set; } // Primary key
        public string EventName { get; set; } = string.Empty; // Not Nullable
        public DateTime EventDate { get; set; }

        // Navigation property
        public virtual ICollection<GuestBooking> GuestBookings { get; set; } = new List<GuestBooking>();
        public virtual ICollection<Staffing> Staffings { get; set; } = new List<Staffing>();

        //// 1-to-0..1 relationship with FoodBooking
        //public int? FoodBookingId { get; set; } // Foreign key to FoodBooking
        //public FoodBooking? FoodBooking { get; set; } // Navigation property to FoodBooking

        // Nullable foreign key to Reservation (implicitly defined)
        public string? ReservationReference { get; set; } // // Foreign key to Reservation

    }
}
