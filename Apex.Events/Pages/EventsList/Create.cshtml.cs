using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        [BindProperty]
        public string SelectedEventTypeId { get; set; } = string.Empty;

        [BindProperty]
        public bool VenuesLoaded { get; set; } = false;

        public List<EventTypeDTO> EventTypes { get; set; } = new();
        public List<VenueDto> AvailableVenues { get; set; } = new();
        public SelectList EventTypeSelectList { get; set; } = new SelectList(new List<EventTypeDTO>(), "Id", "Title");

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadEventTypesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostLoadVenuesAsync()
        {
            await LoadEventTypesAsync();

            if (string.IsNullOrEmpty(SelectedEventTypeId))
            {
                ModelState.AddModelError("", "Please select an event type.");
                return Page();
            }

            if (SelectedDate < DateTime.Today)
            {
                ModelState.AddModelError("", "Event date must be today or later.");
                return Page();
            }

            AvailableVenues = await _venueReservationService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
            VenuesLoaded = true;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadEventTypesAsync();

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogWarning($"{key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            if (string.IsNullOrEmpty(SelectedVenueCode))
            {
                ModelState.AddModelError(nameof(SelectedVenueCode), "Please select a venue.");
                VenuesLoaded = true;
                AvailableVenues = await _venueReservationService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            Event.EventDate = SelectedDate;
            Event.EventTypeId = SelectedEventTypeId;
            Event.VenueCode = SelectedVenueCode;

            // Reserve venue
            var reference = await _venueReservationService.ReserveVenue(SelectedDate, SelectedVenueCode);
            if (reference == null)
            {
                ModelState.AddModelError("", "Failed to reserve the selected venue.");
                VenuesLoaded = true;
                AvailableVenues = await _venueReservationService.GetAvailableVenues(SelectedDate, SelectedEventTypeId);
                return Page();
            }

            Event.ReservationReference = reference;

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Event '{Event.EventName}' created successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadEventTypesAsync()
        {
            try
            {
                EventTypes = await _eventTypeService.GetEventTypesAsync();
            }
            catch
            {
                EventTypes = new List<EventTypeDTO>
                {
                    new EventTypeDTO { Id = "CON", Title = "Conference" },
                    new EventTypeDTO { Id = "MET", Title = "Meeting" },
                    new EventTypeDTO { Id = "PTY", Title = "Party" },
                    new EventTypeDTO { Id = "WED", Title = "Wedding" }
                };
            }

            EventTypeSelectList = new SelectList(EventTypes, "Id", "Title");
        }
    }
}
