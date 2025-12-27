using Apex.Events.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Events.Services
{
    public interface IVenueReservationService
    {
        Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType);
        Task<string?> ReserveVenue(DateTime eventDate, string venueCode);
        Task<bool> FreeReservation(string reference);
    }
}
