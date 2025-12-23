// Apex.Events/Services/IVenueReservationService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Events.Models;

namespace Apex.Events.Services
{
    public interface IVenueReservationService
    {
        Task<string?> ReserveVenue(DateTime eventDate, string venueCode);
        Task<bool> FreeReservation(string reference);
        Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType);
        Task<List<VenueAvailabilityDto>> GetVenueAvailability(DateTime date, string eventType);

        // New method to check multiple dates
        Task<Dictionary<DateTime, List<VenueDto>>> CheckMultipleDates(List<DateTime> dates, string eventType);
    }

    public class VenueAvailabilityDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public double CostPerHour { get; set; }
    }
}