using System.ComponentModel.DataAnnotations;

namespace Apex.Events.Models
{
    public class VenueDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}