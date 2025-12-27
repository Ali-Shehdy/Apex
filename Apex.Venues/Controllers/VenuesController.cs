using Microsoft.AspNetCore.Mvc;
using Apex.Venues.Models; // Your Venue & Availability models
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Venues.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenueController : ControllerBase
    {
        // Mock data for testing purposes
        private static readonly List<Venue> Venues = new()
        {
            new Venue { Code = "CRKHL", Name = "Crackle Hall", Capacity = 100 },
            new Venue { Code = "TNDMR", Name = "Tundra Manor", Capacity = 50 },
            new Venue { Code = "FDLCK", Name = "Fiddle Creek", Capacity = 200 }
        };

        // Mock reservations: Key = date + venueCode, Value = reserved or not
        private static readonly Dictionary<string, bool> Reservations = new();

        // GET api/venue/availability?eventType=MET&beginDate=2026-02-26
        [HttpGet("availability")]
        public IActionResult GetAvailability([FromQuery] string eventType, [FromQuery] DateTime beginDate)
        {
            if (string.IsNullOrEmpty(eventType))
                return BadRequest("Event type is required");

            // Filter venues based on event type (simple rules for testing)
            var allowedVenues = eventType switch
            {
                "MET" => Venues.Where(v => v.Code == "TNDMR").ToList(),
                "CON" => Venues.Where(v => v.Code == "CRKHL" || v.Code == "TNDMR").ToList(),
                "WED" => Venues.Where(v => v.Code == "CRKHL" || v.Code == "TNDMR" || v.Code == "FDLCK").ToList(),
                "PTY" => Venues.Where(v => v.Code == "CRKHL" || v.Code == "FDLCK").ToList(),
                _ => new List<Venue>()
            };

            // Remove already reserved venues
            var availableVenues = allowedVenues.Where(v =>
                !Reservations.ContainsKey($"{beginDate:yyyy-MM-dd}_{v.Code}")).ToList();

            return Ok(availableVenues);
        }

        // POST api/venue/reserve
        [HttpPost("reserve")]
        public IActionResult ReserveVenue([FromBody] ReservationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.VenueCode))
                return BadRequest("Invalid reservation request");

            var key = $"{request.EventDate:yyyy-MM-dd}_{request.VenueCode}";
            if (Reservations.ContainsKey(key))
                return Conflict("Venue already reserved");

            Reservations[key] = true;

            return Ok(new { Reference = Guid.NewGuid().ToString() });
        }

        // DELETE api/venue/reserve/{reference}
        [HttpDelete("reserve/{reference}")]
        public IActionResult FreeReservation(string reference)
        {
            // For simplicity, clear all reservations (mock)
            Reservations.Clear();
            return Ok(true);
        }
    }

    // Models for testing
    public class Venue
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    public class ReservationRequest
    {
        public DateTime EventDate { get; set; }
        public string VenueCode { get; set; } = string.Empty;
    }
}
