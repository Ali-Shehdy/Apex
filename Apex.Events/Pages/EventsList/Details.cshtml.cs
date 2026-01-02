using Apex.Events.Data;
using Apex.Events.Services;
using Apex.Events.Models;
using Apex.Venues.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Events.EventsList
{
    public class DetailsModel : PageModel
    {
        private readonly Apex.Events.Data.EventsDbContext _context;
        private readonly IVenueReservationService _venueReservationService;

        public DetailsModel(Apex.Events.Data.EventsDbContext context, IVenueReservationService venueReservationService)
        {
            _context = context;
            _venueReservationService = venueReservationService;
        }

        public Event Event { get; set; } = default!;
        public ReservationGetDto? Reservation { get; set; }
        public VenueDto? VenueDetails { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(e => e.GuestBookings)
                .ThenInclude(gb => gb.Guest)
                .Include(e => e.Staffings)
                .ThenInclude(s => s.Staff)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (events == null)
            {
                return NotFound();
            }
            else
            {
                Event = events;
            }

            var venueReservationService = HttpContext.RequestServices.GetService<IVenueReservationService>();
            if (venueReservationService != null && !string.IsNullOrWhiteSpace(Event.ReservationReference))
            {
                Reservation = await venueReservationService.GetReservationDetails(Event.ReservationReference);
                VenueDetails = Reservation?.Venue;
            }

            if (venueReservationService != null
                && VenueDetails == null
                && !string.IsNullOrWhiteSpace(Event.VenueCode)
                && !string.IsNullOrWhiteSpace(Event.EventTypeId))
            {
                var availableVenues = await venueReservationService.GetAvailableVenues(Event.EventDate, Event.EventTypeId);
                VenueDetails = availableVenues.FirstOrDefault(v => v.Code == Event.VenueCode);
            }
            return Page();
        }
    }
}
