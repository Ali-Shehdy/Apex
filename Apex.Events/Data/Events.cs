//using Apex.Catering.Data;
//using Apex.Venues.Data;


using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Apex.Events.Data; 
namespace Apex.Events.Data
{
    public class Event
    {
        [Key]
        public int EventId { get; set; } // Primary key
        public string EventName { get; set; } = string.Empty; // Not Nullable
        public DateTime EventDate { get; set; }
        public string? EventTypeId { get; set; }

        // Navigation property
        public virtual ICollection<GuestBooking> GuestBookings { get; set; } = new List<GuestBooking>();
        public virtual ICollection<Staffing> Staffings { get; set; } = new List<Staffing>();

        // Nullable foreign key to Reservation (implicitly defined)
        public string? ReservationReference { get; set; } // // Foreign key to Reservation
        public string? VenueCode { get; set; } // Foreign key to Venue

    }

  
}
