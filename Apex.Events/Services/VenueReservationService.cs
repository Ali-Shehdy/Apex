using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Apex.Events.Models;

namespace Apex.Events.Services
{
    public class VenueReservationService : IVenueReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VenueReservationService> _logger;

        public VenueReservationService(HttpClient httpClient, ILogger<VenueReservationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType)
        {
            // --- Step 2a: Check if eventType is valid ---
            if (string.IsNullOrEmpty(eventType))
                return new List<VenueDto>();

            // --- Step 2b: Use mock/fallback venues ---
            var mockVenues = new List<VenueDto>
    {
        new VenueDto { Code = "CRKHL", Name = "Cork Hall", Capacity = 200 },
        new VenueDto { Code = "TNDMR", Name = "Tandem Room", Capacity = 50 },
        new VenueDto { Code = "FDLCK", Name = "Fieldlock Hall", Capacity = 150 }
    };

            // Optional: filter mock venues based on eventType rules
            if (eventType == "MET") mockVenues = mockVenues.Where(v => v.Code == "TNDMR").ToList();
            if (eventType == "CON") mockVenues = mockVenues.Where(v => v.Code == "CRKHL" || v.Code == "TNDMR").ToList();
            if (eventType == "WED") mockVenues = mockVenues.Where(v => v.Code == "CRKHL" || v.Code == "TNDMR" || v.Code == "FDLCK").ToList();
            if (eventType == "PTY") mockVenues = mockVenues.Where(v => v.Code == "CRKHL" || v.Code == "FDLCK").ToList();

            // --- Step 2c: Optional: try real API call ---
            try
            {
                var url = $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var apiVenues = await response.Content.ReadFromJsonAsync<List<VenueAvailabilityDto>>(options);

                    if (apiVenues != null)
                    {
                        return apiVenues.Select(a => new VenueDto
                        {
                            Code = a.Code,
                            Name = a.Name,
                            Capacity = a.Capacity,
                            Description = a.Description
                        }).ToList();
                    }
                }
            }
            catch
            {
                _logger.LogWarning("Apex.Venues API unavailable. Using mock venues.");
            }

            // Return mock venues if API fails
            return mockVenues;
        }


        // ✅ IMPLEMENT interface method
        public async Task<List<VenueAvailabilityDto>> GetVenueAvailability(DateTime date, string eventType)
        {
            if (string.IsNullOrEmpty(eventType))
                return new List<VenueAvailabilityDto>();

            try
            {
                var url = $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<VenueAvailabilityDto>();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = await response.Content.ReadFromJsonAsync<List<VenueAvailabilityDto>>(options);
                return result ?? new List<VenueAvailabilityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting venue availability");
                return new List<VenueAvailabilityDto>();
            }
        }

        public async Task<string?> ReserveVenue(DateTime eventDate, string venueCode)
        {
            try
            {
                var dto = new ReservationPostDto { EventDate = eventDate, VenueCode = venueCode };
                var response = await _httpClient.PostAsJsonAsync("api/reservations", dto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to reserve venue. Status: {response.StatusCode}, Error: {error}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ReservationGetDto>();
                return result?.Reference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving venue");
                return null;
            }
        }

        public async Task<bool> FreeReservation(string reference)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/reservations/{reference}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error freeing reservation");
                return false;
            }
        }

        public async Task<Dictionary<DateTime, List<VenueDto>>> CheckMultipleDates(List<DateTime> dates, string eventType)
        {
            var results = new Dictionary<DateTime, List<VenueDto>>();
            foreach (var date in dates)
            {
                results[date] = await GetAvailableVenues(date, eventType);
            }
            return results;
        }
    }
}
