namespace Apex.Events.Data
{
    public class Guest
    {
        public int GuestId { get; set; } // Primary key
        public string FirstName { get; set; } = string.Empty; // Not Nullable
        public string LastName { get; set; } = string.Empty; // Not Nullable
        public string Email { get; set; } = string.Empty; // Not Nullable
        // Navigation property
        public virtual ICollection<GuestBooking> GuestBookings { get; set; } = new List<GuestBooking>();
    }
}
