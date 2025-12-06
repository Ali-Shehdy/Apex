using System.Net.Http;
using System.Net.Http.Json;
using Apex.Venues.Models;

public class VenueService
{
    private readonly HttpClient _http;

    public VenueService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<VenueDto>> GetAvailableVenues(DateTime date)
    {
        return await _http.GetFromJsonAsync<List<VenueDto>>(
            $"availabilities/{date:yyyy-MM-dd}"
        ) ?? new List<VenueDto>();
    }

    public async Task<string?> ReserveVenue(DateTime date, string venueCode)
    {
        var dto = new ReservationPostDto
        {
            EventDate = date,
            VenueCode = venueCode
        };

        var response = await _http.PostAsJsonAsync("reservations", dto);

        if (!response.IsSuccessStatusCode)
            return null;

        var reservation = await response.Content.ReadFromJsonAsync<ReservationGetDto>();
        return reservation?.Reference;
    }

    public async Task FreeReservation(string reference)
    {
        await _http.DeleteAsync($"reservations/{reference}");
    }
}
