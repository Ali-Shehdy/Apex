// Apex.Events/Services/VenueReservationService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Apex.Events.Dto;
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

        // ✅ METHOD 1: Get available venues filtered by event type (CALLS APEX.VENUES API)
        public async Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType)
        {
            try
            {
                if (string.IsNullOrEmpty(eventType))
                {
                    _logger.LogWarning("Event type is required to fetch available venues");
                    return new List<VenueDto>();
                }

                // Call Apex.Venues Availability API
                var url = $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}";
                _logger.LogInformation($"Calling Apex.Venues API: {url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug($"API Response: {jsonResponse}");

                    // Deserialize the response
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var availabilityList = await response.Content.ReadFromJsonAsync<List<VenueAvailabilityDto>>(options);

                    // Convert to VenueDto
                    var venues = new List<VenueDto>();
                    if (availabilityList != null)
                    {
                        foreach (var availability in availabilityList)
                        {
                            venues.Add(new VenueDto
                            {
                                Code = availability.Code,
                                Name = availability.Name,
                                Description = availability.Description,
                                Capacity = availability.Capacity
                            });
                        }
                    }


                    _logger.LogInformation($"Found {venues.Count} available venues for {eventType} on {date:yyyy-MM-dd}");
                    return venues;
                }
                else
                {
                    _logger.LogError($"Failed to get venues. Status: {response.StatusCode}");
                    return new List<VenueDto>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling Apex.Venues API");
                return new List<VenueDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available venues");
                return new List<VenueDto>();
            }
        }

        // ✅ METHOD 2: Get venue availability with all details
        public async Task<List<VenueAvailabilityDto>> GetVenueAvailability(DateTime date, string eventType)
        {
            try
            {
                if (string.IsNullOrEmpty(eventType))
                {
                    return new List<VenueAvailabilityDto>();
                }

                var url = $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = await response.Content.ReadFromJsonAsync<List<VenueAvailabilityDto>>(options);
                    return result ?? new List<VenueAvailabilityDto>();
                }

                return new List<VenueAvailabilityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting venue availability");
                return new List<VenueAvailabilityDto>();
            }
        }

        // ✅ METHOD 3: Reserve venue
        public async Task<string?> ReserveVenue(DateTime eventDate, string venueCode)
        {
            try
            {
                var reservationDto = new ReservationPostDto
                {
                    EventDate = eventDate,
                    VenueCode = venueCode
                };

                var response = await _httpClient.PostAsJsonAsync("api/reservations", reservationDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReservationGetDto>();
                    return result?.Reference;
                }

                _logger.LogError($"Failed to reserve venue. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving venue");
                return null;
            }
        }

        // ✅ METHOD 4: Free reservation
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