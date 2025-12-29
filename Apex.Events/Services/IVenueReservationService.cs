using EventsVenueDto = Apex.Events.Models.VenueDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Events.Services
{
    public interface IVenueReservationService
    {
        /// <summary>
        /// Gets available venues for the specified date and event type.
        /// </summary>
        Task<List<EventsVenueDto>> GetAvailableVenues(DateTime date, string eventType);

        /// <summary>
        /// Reserves a venue for the specified event date and venue code.
        /// </summary>
        Task<string?> ReserveVenue(DateTime eventDate, string venueCode);

        /// <summary>
        /// Releases a reservation by reference.
        /// </summary>
        Task<bool> FreeReservation(string reference);

    }
}