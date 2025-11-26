using System.ComponentModel.DataAnnotations;
using Apex.Events.Data;
namespace Apex.Events.Dto
{
    public class UpdateEventDto
    {

        [Required]
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public EventType EventType { get; set; }
    }
}
