using System;
using System.ComponentModel.DataAnnotations;

namespace Apex.Events.Models
{
    public class ReservationGetDto
    {
        [Required]
        [MinLength(13)]
        [MaxLength(13)]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(5)]
        public string VenueCode { get; set; } = string.Empty;

        public DateTime WhenMade { get; set; }

        public VenueDto Venue { get; set; } = new VenueDto();
    }
}