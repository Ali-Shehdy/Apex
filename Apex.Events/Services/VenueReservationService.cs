using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Apex.Events.Models;
using Microsoft.Extensions.Logging;

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
            try
            {
                // Workaround: Apex.Venues compares a.Date <= endDate.Date
                // but seeded availability has a time component. So include next day.
                var endDate = date.Date.AddDays(1);

                var url = $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
                _logger.LogInformation("Calling Venues availability: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Reserve failed: {Status}. Body: {Body}", response.StatusCode, body);
                    return null;
                }

                var venues = await response.Content.ReadFromJsonAsync<List<VenueDto>>() ?? new List<VenueDto>();

                // keep only the selected date
                return venues.Where(v => v.Date.Date == date.Date).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available venues");
                return new List<VenueDto>();
            }
        }

        public async Task<string?> ReserveVenue(DateTime eventDate, string venueCode)
        {
            try
            {
                var request = new ReservationPostDto
                {
                    EventDate = eventDate.Date,
                    VenueCode = venueCode
                };

                var response = await _httpClient.PostAsJsonAsync("api/reservations", request);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("❌ Reserve failed. Status={Status}. Body={Body}", response.StatusCode, body);
                    return null;
                }

                // Apex.Venues returns ReservationGetDto shape (includes Reference)
                var result = await response.Content.ReadFromJsonAsync<ReservationGetDto>();
                _logger.LogInformation("✅ Reserved. Reference={Ref}", result?.Reference);
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
    }
}
