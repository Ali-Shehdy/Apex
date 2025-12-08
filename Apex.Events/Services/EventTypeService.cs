using System.Net.Http;
using System.Net.Http.Json;
using Apex.Events.Models;

namespace Apex.Events.Services
{
    public class EventTypeService
    {
        private readonly HttpClient _client;

        public EventTypeService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<EventTypeDTO>> GetEventTypesAsync()
        {
            try
            {
                var response = await _client.GetAsync("api/eventtypes");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<EventTypeDTO>>();
                    return result ?? new List<EventTypeDTO>();
                }
                else
                {
                    Console.WriteLine($"EventTypeService HTTP Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EventTypeService Exception: {ex.Message}");
            }

            return new List<EventTypeDTO>();
        }
    }
}
