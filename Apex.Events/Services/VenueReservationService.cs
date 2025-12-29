﻿using EventsVenueDto = Apex.Events.Models.VenueDto;
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
        public async Task<List<EventsVenueDto>> GetAvailableVenues(DateTime date, string eventType)
        {
            // Workaround for Apex.Venues date/time filtering bug:
            // include endDate = next day so times within the selected day are included.
            var endDate = date.Date.AddDays(1);

            var url =
                $"api/availability?eventType={eventType}&beginDate={date:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<EventsVenueDto>();

            var venues = await response.Content.ReadFromJsonAsync<List<EventsVenueDto>>()
                         ?? new List<EventsVenueDto>();

            // Keep only venues for the selected day
            return venues.Where(v => v.Date.Date == date.Date).ToList();
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

                var response = await _httpClient.PostAsJsonAsync("api/reservations", request);
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
                var response = await _httpClient.DeleteAsync($"api/reservations/{reference}");
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