using Apex.Events.Models;

public class EventTypeService
{
    private readonly HttpClient _client;

    public EventTypeService(HttpClient client)
    {
        _client = client;  // BaseAddress comes from Program.cs
    }

    public async Task<List<EventTypeDTO>> GetEventTypesAsync()
    {
        try
        {
            var response = await _client.GetAsync("api/eventtypes");

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<List<EventTypeDTO>>()
                ?? new List<EventTypeDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API ERROR: {ex.Message}");
            return new List<EventTypeDTO>();
        }
    }
}
