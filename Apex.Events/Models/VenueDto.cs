namespace Apex.Events.Models
{
    public class VenueDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public double CostPerHour { get; set; }
    }
}
