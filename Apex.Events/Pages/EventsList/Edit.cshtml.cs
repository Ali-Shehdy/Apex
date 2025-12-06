using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.EventsList
{
    public class EditModel : PageModel
    {
        private readonly EventsDbContext _context;
        private readonly VenueService _venueService;

        public EditModel(EventsDbContext context, VenueService venueService)
        {
            _context = context;
            _venueService = venueService;
        }

        [BindProperty]
        public Apex.Events.Data.Event Event { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var events = await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);

            if (events == null)
                return NotFound();

            Event = events;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Load the original event (we must keep the date & type)
            var existingEvent = await _context.Events.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == Event.EventId);

            if (existingEvent == null)
                return NotFound();

            // Keep original values
            Event.EventDate = existingEvent.EventDate;
            Event.EventType = existingEvent.EventType;

            // --- STEP 4: Update Reservation via Apex.Venues ---
            string? reservationReference = await _venueService.ReserveVenue(
                existingEvent.EventDate,
                Event.VenueCode!
            );

            if (reservationReference == null)
            {
                ModelState.AddModelError("", "Venue reservation failed.");
                return Page();
            }

            // Update event with new reservation reference
            Event.ReservationReference = reservationReference;

            _context.Attach(Event).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
