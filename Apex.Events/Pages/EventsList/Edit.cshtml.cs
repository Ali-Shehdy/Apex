// Apex.Events/Pages/EventsList/Edit.cshtml.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using Apex.Events.Services;

namespace Apex.Events.EventsList
{
    public class EditModel : PageModel
    {
        private readonly EventsDbContext _context;
        private readonly IVenueReservationService _venueReservationService;

        public EditModel(EventsDbContext context, IVenueReservationService venueReservationService)
        {
            _context = context;
            _venueReservationService = venueReservationService;
        }

        [BindProperty]
        public Data.Event Event { get; set; } = default!;

        // Add property for venue selection
        [BindProperty]
        public string? SelectedVenueCode { get; set; }

        // Change the type of AvailableVenues to match the return type of GetAvailableVenues
        public List<Apex.Events.Models.VenueDto> AvailableVenues { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var eventItem = await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);

            if (eventItem == null)
                return NotFound();

            Event = eventItem;
            SelectedVenueCode = Event.VenueCode;

            // Get available venues for the event date
            AvailableVenues = await _venueReservationService.GetAvailableVenues(Event.EventDate, Event.EventTypeId ?? string.Empty);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload available venues on validation error
                AvailableVenues = await _venueReservationService.GetAvailableVenues(Event.EventDate, Event.EventTypeId ?? string.Empty);
                return Page();
            }

            // Load the original event
            var existingEvent = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == Event.EventId);

            if (existingEvent == null)
                return NotFound();

            // Keep original date
            Event.EventDate = existingEvent.EventDate;

            // If venue code changed, make new reservation
            if (!string.IsNullOrEmpty(SelectedVenueCode) && SelectedVenueCode != existingEvent.VenueCode)
            {
                // Free old reservation if exists
                if (!string.IsNullOrEmpty(existingEvent.ReservationReference))
                {
                    await _venueReservationService.FreeReservation(existingEvent.ReservationReference);
                }

                // Make new reservation
                string? reservationReference = await _venueReservationService.ReserveVenue(
                    Event.EventDate,
                    SelectedVenueCode
                );

                if (reservationReference == null)
                {
                    ModelState.AddModelError("", "Failed to reserve venue. Please try again.");
                    AvailableVenues = await _venueReservationService.GetAvailableVenues(Event.EventDate, Event.EventTypeId ?? string.Empty);
                    return Page();
                }

                Event.VenueCode = SelectedVenueCode;
                Event.ReservationReference = reservationReference;
            }
            else
            {
                // Keep existing venue and reservation
                Event.VenueCode = existingEvent.VenueCode;
                Event.ReservationReference = existingEvent.ReservationReference;
            }

            _context.Attach(Event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Events.AnyAsync(e => e.EventId == Event.EventId))
                    return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}