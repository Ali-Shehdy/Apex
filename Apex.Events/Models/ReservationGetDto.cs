namespace Apex.Events.Models
{
    public class ReservationGetDto
    {
        public string Reference { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string VenueCode { get; set; } = "";

        // These may be included depending on Venues DTO; harmless if ignored
        public string? VenueName { get; set; }
        public string? VenueDescription { get; set; }
        public int Capacity { get; set; }
    }
}
