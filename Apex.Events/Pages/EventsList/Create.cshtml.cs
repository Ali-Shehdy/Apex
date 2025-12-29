using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Apex.Events.Data;
using Apex.Events.Models;
using Apex.Events.Services;

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
            await LoadEventTypes();

            if (string.IsNullOrEmpty(SelectedEventTypeId))
            {
                ModelState.AddModelError("", "Please select an event type.");
                return Page();
            }

            if (SelectedDate < DateTime.Today)
            {
                ModelState.AddModelError("", "Event date cannot be in the past.");
                return Page();
            }

            AvailableVenues = await _venueService
                .GetAvailableVenues(SelectedDate, SelectedEventTypeId);

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
            await LoadEventTypes();

            if (!ModelState.IsValid)
                return Page();

            if (string.IsNullOrEmpty(SelectedVenueCode))
            {
                ModelState.AddModelError("", "Please select a venue.");
                VenuesLoaded = true;
                return Page();
            }

            Event.EventDate = SelectedDate;
            Event.EventTypeId = SelectedEventTypeId;
            Event.VenueCode = SelectedVenueCode;

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Event created successfully.";
            return RedirectToPage("./Index");
        }

        // =======================
        // HELPERS
        // =======================

        private async Task LoadEventTypes()
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();
            EventTypeSelectList = new SelectList(EventTypes, "Id", "Title");
        }
    }
}
