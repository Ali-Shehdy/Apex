using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Apex.Catering.Data
{
    public class FoodBooking
    {

        [Key]
        public int FoodBookingId { get; set; } // Primary key

<<<<<<< HEAD
        public int? ClintReferenceId { get; set; } // Nullable 
        public int? NumberOfGuests { get; set; } // Nullable
        public int? MenuId { get; set; } // Foreign key to Menu
=======
        public int ClintReferenceId { get; set; } // Nullable 
        public int NumberOfGuests { get; set; } // Nullable
        public int MenuId { get; set; } // Foreign key to Menu
>>>>>>> f1a6b557f5735b139d681b5cf498b2be3643c127

        public virtual Menu? Menu { get; set; } // Navigation property to Menu

    }
}
