using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Events.Models;

namespace Apex.Events.Services
{
    public interface IVenueReservationService
    {
        Task<List<VenueDto>> GetAvailableVenues(DateTime date, string eventType);

        // returns reservation reference string like "TNDMR20260306"
        Task<string?> ReserveVenue(DateTime eventDate, string venueCode);

        Task<bool> FreeReservation(string reference);

        Task<ReservationGetDto?> GetReservationDetails(string reference);
    }
}
