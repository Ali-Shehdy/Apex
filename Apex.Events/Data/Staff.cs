namespace Apex.Events.Data
{
    public class Staff
    {
        public int StaffId { get; set; } // Primary key
        public string FirstName { get; set; } = string.Empty; // Not Nullable
        public string LastName { get; set; } = string.Empty; // Not Nullable
        public string Role { get; set; } = string.Empty; // Not Nullable
        // Navigation property
        public virtual ICollection<Staffing> Staffings { get; set; } = new List<Staffing>();
    }
}
