using Apex.Events.Data;
using Apex.Events.Services;
using Apex.Events.Models;
using Apex.Venues.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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


            if (!string.IsNullOrWhiteSpace(Event.ReservationReference))
            {
                Reservation = await _venueReservationService.GetReservationDetails(Event.ReservationReference);
            }
            return Page();
        }
    }
}
