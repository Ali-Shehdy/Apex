namespace Apex.Events.Data
{
    public class Staffing
    {
        public int StaffingId { get; set; } // Primary key
        public int EventId { get; set; } // Foreign key to Event
        public int StaffId { get; set; } // Foreign key to Staff


        public Staff Staff { get; set; } = null!; // Relation property to Staff
        public Event Event { get; set; } = null!; // Relation property to Event

    }
}
