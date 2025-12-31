using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Events.Data;          // EF entity Event + DbContext
using Apex.Events.Models;        // DTOs: EventTypeDTO, VenueDto
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
        public Event Event { get; set; } = new();

        [BindProperty]
        public string SelectedEventTypeId { get; set; } = string.Empty;

        [BindProperty]
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        [BindProperty]
        public string SelectedVenueCode { get; set; } = string.Empty;

        // STEP A: bind this so the dropdown & debug do not disappear after a failed POST
        [BindProperty]
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
            TempData["DebugCreate"] = "STEP LV1: OnPostLoadVenues fired ✅";

            // Not creating yet; do not require venue selection here
            ModelState.Remove(nameof(SelectedVenueCode));

            await LoadEventTypes();

            if (string.IsNullOrWhiteSpace(SelectedEventTypeId))
            {
                TempData["DebugCreate"] = "STEP LV2: Missing event type ❌";
                ModelState.AddModelError("", "Please select an event type.");
                return Page();
            }

            if (SelectedDate.Date < DateTime.Today)
            {
                TempData["DebugCreate"] = "STEP LV3: Date in the past ❌";
                ModelState.AddModelError("", "Event date cannot be in the past.");
                return Page();
            }

            AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
            VenuesLoaded = true;

            TempData["DebugCreate"] = $"STEP LV4: Loaded venues ✅ Count={AvailableVenues.Count}";

            if (!AvailableVenues.Any())
            {
                ModelState.AddModelError("", "No venues available for this date and event type.");
            }

            return Page();
        }

        // =======================
        // CREATE EVENT
        // =======================

        public async Task<IActionResult> OnPostCreateAsync()
        {
            TempData["DebugCreate"] = "STEP 1: Entered OnPostCreateAsync ✅";

            _logger.LogInformation(
                "Create POST: Name={Name}, Type={Type}, Date={Date}, Venue={Venue}",
                Event?.EventName,
                SelectedEventTypeId,
                SelectedDate.ToString("yyyy-MM-dd"),
                SelectedVenueCode
            );

            await LoadEventTypes();

            // STEP 2: Ignore binding-time validation errors for fields we set server-side
            ModelState.Remove("Event.EventTypeId");
            ModelState.Remove("Event.VenueCode");
            ModelState.Remove("Event.EventDate");
            ModelState.Remove("Event.ReservationReference");

            // Manual validation (source of truth = Selected* fields)
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
                TempData["DebugCreate"] = "STEP 2: ModelState invalid ❌ (returned Page)";
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            TempData["DebugCreate"] = "STEP 3: Valid ✅ About to reserve venue";

            // 1) Reserve in Apex.Venues first
            var reference = await _venueService.ReserveVenue(SelectedDate, SelectedVenueCode);

            if (string.IsNullOrWhiteSpace(reference))
            {
                TempData["DebugCreate"] = "STEP 4: ReserveVenue FAILED ❌ (returned Page)";
                ModelState.AddModelError("", "Failed to reserve venue. Please choose another venue/date.");
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            TempData["DebugCreate"] = $"STEP 5: Reserve OK ✅ Ref={reference}. About to save event";

            // 2) Save in Apex.Events
            Event.EventDate = SelectedDate.Date;
            Event.EventTypeId = SelectedEventTypeId;
            Event.VenueCode = SelectedVenueCode;
            Event.ReservationReference = reference;

            _context.Events.Add(Event);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["DebugCreate"] = "STEP 6: SaveChanges FAILED ❌ (returned Page)";
                _logger.LogError(ex, "DB save failed; freeing reservation {Ref}", reference);
                await _venueService.FreeReservation(reference);

                ModelState.AddModelError("", "Database error while saving the event.");
                VenuesLoaded = true;
                AvailableVenues = await _venueService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            TempData["Success"] = "Event created successfully.";
            TempData["DebugCreate"] = "STEP 7: Saved ✅ Redirecting to Index";

            return RedirectToPage("./Index");
        }

        private async Task LoadEventTypes()
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();
            EventTypeSelectList = new SelectList(EventTypes, "Id", "Title");
        }
    }
}
