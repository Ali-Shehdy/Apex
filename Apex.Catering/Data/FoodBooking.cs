using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Apex.Catering.Data
{
    public class FoodBooking
    {

        [Key]
        public int FoodBookingId { get; set; } // Primary key

        public int ClintReferenceId { get; set; } // Nullable 
        public int NumberOfGuests { get; set; } // Nullable
        public int MenuId { get; set; } // Foreign key to Menu

        public virtual Menu? Menu { get; set; } // Navigation property to Menu

    }
}
