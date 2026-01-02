using Apex.Events.Data; // Ensure this using directive is present
using Apex.Events.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apex.Events.EventsList
{
    public class DeleteModel : PageModel
    {
        private readonly Apex.Events.Data.EventsDbContext _context;
        private readonly IVenueReservationService _venueReservationService;

        public DeleteModel(Apex.Events.Data.EventsDbContext context, IVenueReservationService venueReservationService)
        {
            _context = context;
            _venueReservationService = venueReservationService;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);

            if (events == null)
            {
                return NotFound();
            }
            else
            {
                Event = events;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FindAsync(id);
            if (events != null)
            {
                Event = events;
                var staffings = await _context.Staffings
                                   .Where(s => s.EventId == Event.EventId)
                                   .ToListAsync();

                if (staffings.Count > 0)
                {
                    _context.Staffings.RemoveRange(staffings);
                }

                if (!string.IsNullOrWhiteSpace(Event.ReservationReference))
                {
                    await _venueReservationService.FreeReservation(Event.ReservationReference);
                }

                Event.ReservationReference = null;
                Event.VenueCode = null;
                Event.IsCancelled = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
