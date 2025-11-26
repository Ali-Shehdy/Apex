using Apex.Events.Data;
using System.ComponentModel.DataAnnotations;


namespace Apex.Events.Dto
{
    public class CreateEventDto
    {

        [Required]
        public string EventName { get; set; } = string.Empty;
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public EventType EventType { get; set; }

    }
}
