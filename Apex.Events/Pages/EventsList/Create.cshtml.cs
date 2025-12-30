using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Events.Data;          // ✅ EF entity Event + DbContext
using Apex.Events.Models;        // ✅ DTOs: EventTypeDTO, VenueDto
using Apex.Events.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Apex.Events.Pages.EventsList
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;
        private readonly EventTypeService _eventTypeService;
        private readonly IVenueReservationService _venueService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            EventsDbContext context,
            EventTypeService eventTypeService,
            IVenueReservationService venueService,
            ILogger<CreateModel> logger)
        {
            _context = context;
            _eventTypeService = eventTypeService;
            _venueService = venueService;
            _logger = logger;
        }

        // =======================
        // BIND PROPERTIES
        // =======================

        [BindProperty]
        public Event Event { get; set; } = new(); // ✅ IMPORTANT: Apex.Events.Data.Event

        [BindProperty]
        public string SelectedEventTypeId { get; set; } = string.Empty;

        [BindProperty]
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        [BindProperty]
        public string SelectedVenueCode { get; set; } = string.Empty;

        public bool VenuesLoaded { get; set; }

        // =======================
        // VIEW DATA
        // =======================

        public List<EventTypeDTO> EventTypes { get; set; } = new();
        public List<VenueDto> AvailableVenues { get; set; } = new();
        public SelectList EventTypeSelectList { get; set; } = default!;

        // =======================
        // GET
        // =======================

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadEventTypes();
            return Page();
        }

        // =======================
        // LOAD AVAILABLE VENUES
        // =======================

        public async Task<IActionResult> OnPostLoadVenuesAsync()
        {
            // We're not creating yet; don't validate SelectedVenueCode as required here.
            ModelState.Remove(nameof(SelectedVenueCode));

            await LoadEventTypes();

            if (string.IsNullOrWhiteSpace(SelectedEventTypeId))
            {
                ModelState.AddModelError("", "Please select an event type.");
                return Page();
            }

            if (SelectedDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Event date cannot be in the past.");
                return Page();
            }

            AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);

            VenuesLoaded = true;

            if (!AvailableVenues.Any())
            {
                ModelState.AddModelError("", "No venues available for this date and event type.");
            }

            return Page();
        }

        // =======================
        // CREATE EVENT
        // =======================

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation(
                "🔥 OnPostAsync fired. Name={Name}, Type={Type}, Date={Date}, Venue={Venue}",
                Event?.EventName,
                SelectedEventTypeId,
                SelectedDate.ToString("yyyy-MM-dd"),
                SelectedVenueCode
            );

            TempData["DebugCreate"] =
                $"POST fired: {Event?.EventName} / {SelectedEventTypeId} / {SelectedDate:yyyy-MM-dd} / {SelectedVenueCode}";

            await LoadEventTypes();

            // Basic checks
            if (string.IsNullOrWhiteSpace(Event?.EventName))
                ModelState.AddModelError("", "Event name is required.");

            if (string.IsNullOrWhiteSpace(SelectedEventTypeId))
                ModelState.AddModelError("", "Please select an event type.");

            if (SelectedDate.Date < DateTime.Today)
                ModelState.AddModelError("", "Event date cannot be in the past.");

            if (string.IsNullOrWhiteSpace(SelectedVenueCode))
                ModelState.AddModelError("", "Please select a venue.");

            if (!ModelState.IsValid)
            {
                // keep dropdown visible and filled
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            // 1) Reserve in Apex.Venues first
            var reference = await _venueService.ReserveVenue(SelectedDate, SelectedVenueCode);

            if (string.IsNullOrWhiteSpace(reference))
            {
                ModelState.AddModelError("", "Failed to reserve venue. Please choose another venue/date.");
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            // 2) Save in Apex.Events
            Event.EventDate = SelectedDate.Date;
            Event.EventTypeId = SelectedEventTypeId;
            Event.VenueCode = SelectedVenueCode;
            Event.ReservationReference = reference; // ✅ this links your event to Venues reservation

            _context.Events.Add(Event);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Roll back the reservation if local save fails
                _logger.LogError(ex, "DB save failed; freeing reservation {Ref}", reference);
                await _venueService.FreeReservation(reference);

                ModelState.AddModelError("", "Database error while saving the event.");
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            TempData["Success"] = "Event created successfully.";
            return RedirectToPage("./Index");
        }

        private async Task LoadEventTypes()
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();
            EventTypeSelectList = new SelectList(EventTypes, "Id", "Title");
        }
    }
}
