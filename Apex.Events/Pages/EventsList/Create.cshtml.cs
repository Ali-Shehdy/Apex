// Apex.Events/Pages/EventsList/Create.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using Apex.Events.Services;
using Apex.Events.Models;

namespace Apex.Events.EventsList
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;
        private readonly EventTypeService _eventTypeService;
        private readonly IVenueReservationService _venueReservationService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            EventsDbContext context,
            EventTypeService eventTypeService,
            IVenueReservationService venueReservationService,
            ILogger<CreateModel> logger)
        {
            _context = context;
            _eventTypeService = eventTypeService;
            _venueReservationService = venueReservationService;
            _logger = logger;
        }

        [BindProperty]
        public Event Event { get; set; } = new Event();

        [BindProperty]
        public string SelectedVenueCode { get; set; } = string.Empty;

        [BindProperty]
        public DateTime SelectedDate { get; set; }

        [BindProperty]
        public string SelectedEventTypeId { get; set; } = string.Empty;

        public List<EventTypeDTO> EventTypes { get; set; } = new();
        public List<VenueDto> AvailableVenues { get; set; } = new();

        public SelectList EventTypeSelectList { get; set; }
        public SelectList VenueSelectList { get; set; }

        public bool HasCheckedVenues { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // ✅ FIX: Load EventTypes on GET
                await LoadEventTypes();

                // Set default date
                Event.EventDate = DateTime.Today.AddDays(1);
                SelectedDate = Event.EventDate;

                _logger.LogInformation("Create page loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create page");
                ModelState.AddModelError("", "Error loading page. Please try again.");
            }

            return Page();
        }

        // Handler for when event type OR date changes
        public async Task<IActionResult> OnPostLoadVenuesAsync()
        {
            try
            {
                // Reload event types
                await LoadEventTypes();

                // ✅ Validate: Event type is required
                if (string.IsNullOrEmpty(SelectedEventTypeId))
                {
                    ModelState.AddModelError("SelectedEventTypeId", "Please select an event type first.");
                    return Page();
                }

                // ✅ Validate: Date is required
                if (SelectedDate < DateTime.Today)
                {
                    ModelState.AddModelError("SelectedDate", "Event date must be today or in the future.");
                    return Page();
                }

                // ✅ CRITICAL: Get available venues from Apex.Venues API
                AvailableVenues = await _venueReservationService.GetAvailableVenues(
                    SelectedDate,
                    SelectedEventTypeId
                );

                // Debug log
                _logger.LogInformation($"Found {AvailableVenues.Count} venues for {SelectedEventTypeId} on {SelectedDate:yyyy-MM-dd}");

                VenueSelectList = new SelectList(AvailableVenues, "Code", "Name");
                HasCheckedVenues = true;

                // Sync Event.EventDate with SelectedDate
                Event.EventDate = SelectedDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading venues");
                ModelState.AddModelError("", "Error checking available venues. Please try again.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadEventTypes();
                return Page();
            }

            // Set event type
            Event.EventTypeId = SelectedEventTypeId;
            Event.EventDate = SelectedDate;

            // Make reservation if venue selected
            if (!string.IsNullOrEmpty(SelectedVenueCode))
            {
                var reservationReference = await _venueReservationService.ReserveVenue(
                    Event.EventDate,
                    SelectedVenueCode
                );

                if (reservationReference == null)
                {
                    ModelState.AddModelError("", "Failed to reserve venue.");
                    await LoadEventTypes();
                    return Page();
                }

                Event.ReservationReference = reservationReference;
                Event.VenueCode = SelectedVenueCode;
            }

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadEventTypes()
        {
            try
            {
                EventTypes = await _eventTypeService.GetEventTypesAsync();

                if (!EventTypes.Any())
                {
                    _logger.LogWarning("No event types returned from API, using fallback");
                    EventTypes = GetFallbackEventTypes();
                }

                EventTypeSelectList = new SelectList(EventTypes, "Id", "Title", SelectedEventTypeId);
                _logger.LogInformation($"Loaded {EventTypes.Count} event types");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event types");
                EventTypes = GetFallbackEventTypes();
                EventTypeSelectList = new SelectList(EventTypes, "Id", "Title", SelectedEventTypeId);
            }
        }

        private List<EventTypeDTO> GetFallbackEventTypes()
        {
            return new List<EventTypeDTO>
            {
                new EventTypeDTO { Id = "CON", Title = "Conference" },
                new EventTypeDTO { Id = "MET", Title = "Meeting" },
                new EventTypeDTO { Id = "PTY", Title = "Party" },
                new EventTypeDTO { Id = "WED", Title = "Wedding" }
            };
        }
    }
}