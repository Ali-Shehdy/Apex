// Apex.Events/Models/ReservationPostDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Apex.Events.Models
{
    public class ReservationPostDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(5)]
        public string VenueCode { get; set; } = string.Empty;
    }
}