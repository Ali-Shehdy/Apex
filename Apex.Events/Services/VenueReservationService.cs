using Apex.Events.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

        // Get available venues from Apex.Venues API
        public async Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType)
        {
            try
            {
                var url = $"api/venue/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to load venues. Status: {response.StatusCode}");
                    return new List<VenueDto>();
                }

                var venues = await response.Content.ReadFromJsonAsync<List<VenueDto>>();
                return venues ?? new List<VenueDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available venues");
                return new List<VenueDto>();
            }
        }

        // Reserve a venue
        public async Task<string?> ReserveVenue(DateTime eventDate, string venueCode)
        {
            try
            {
                var request = new
                {
                    EventDate = eventDate,
                    VenueCode = venueCode
                };

                var response = await _httpClient.PostAsJsonAsync("api/venue/reserve", request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to reserve venue. Status: {response.StatusCode}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ReservationResponse>();
                return result?.Reference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving venue");
                return null;
            }
        }

        // Free a reservation
        public async Task<bool> FreeReservation(string reference)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/venue/reserve/{reference}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error freeing reservation");
                return false;
            }
        }

        // DTO for reservation response
        private class ReservationResponse
        {
            public string? Reference { get; set; }
        }
    }
}
