// Apex.Events/Pages/EventsList/Create.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public DateTime SelectedDate { get; set; }

        [BindProperty]
        public string SelectedEventTypeId { get; set; } = string.Empty;

        public List<EventTypeDTO> EventTypes { get; set; } = new();
        public List<VenueDto> AvailableVenues { get; set; } = new();
        public List<DateTime> SuggestedDates { get; set; } = new();

        public SelectList EventTypeSelectList { get; set; }
        public SelectList VenueSelectList { get; set; }

        public bool HasCheckedVenues { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadEventTypes();
                GenerateSuggestedDates();

                // Set default date to 2 months from now (where Apex.Venues starts)
                SelectedDate = SuggestedDates.FirstOrDefault();
                Event.EventDate = SelectedDate;

                // Initialize select lists
                EventTypeSelectList = new SelectList(EventTypes, "Id", "Title");
                VenueSelectList = new SelectList(AvailableVenues, "Code", "Name");

                _logger.LogInformation("Create page loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create page");
                Message = "Error loading page. Please try again.";
            }

            return Page();
        }

        // NEW: API endpoint for real-time availability checking
        public async Task<JsonResult> OnGetCheckAvailabilityAsync(string eventType, string date)
        {
            try
            {
                _logger.LogInformation($"Checking availability for {eventType} on {date}");

                if (!DateTime.TryParse(date, out var parsedDate))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Invalid date format"
                    });
                }

                if (string.IsNullOrEmpty(eventType))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Event type is required"
                    });
                }

                var availableVenues = await _venueReservationService.GetAvailableVenues(parsedDate, eventType);

                _logger.LogInformation($"Found {availableVenues.Count} available venues for {eventType} on {parsedDate:yyyy-MM-dd}");

                return new JsonResult(new
                {
                    success = true,
                    date = parsedDate.ToString("yyyy-MM-dd"),
                    eventType,
                    availableVenuesCount = availableVenues.Count,
                    availableVenues = availableVenues.Select(v => new
                    {
                        v.Code,
                        v.Name,
                        v.Capacity,
                        v.Description
                    }),
                    lastChecked = DateTime.Now.ToString("HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return new JsonResult(new
                {
                    success = false,
                    error = ex.Message,
                    details = ex.StackTrace
                });
            }
        }

        public async Task<IActionResult> OnPostLoadVenuesAsync()
        {
            try
            {
                await LoadEventTypes();
                GenerateSuggestedDates();

                // Validate inputs
                if (string.IsNullOrEmpty(SelectedEventTypeId))
                {
                    Message = "Please select an event type first.";
                    return Page();
                }

                if (SelectedDate < DateTime.Today)
                {
                    Message = "Event date must be today or in the future.";
                    return Page();
                }

                // Check availability for selected date
                AvailableVenues = await _venueReservationService.GetAvailableVenues(
                    SelectedDate,
                    SelectedEventTypeId
                );

                // If no venues, try nearby dates
                if (!AvailableVenues.Any())
                {
                    _logger.LogInformation($"No venues for {SelectedDate:yyyy-MM-dd}, checking nearby dates...");

                    var nearbyDates = new List<DateTime>
                    {
                        SelectedDate.AddDays(1),
                        SelectedDate.AddDays(-1),
                        SelectedDate.AddDays(2),
                        SelectedDate.AddDays(-2),
                        SelectedDate.AddDays(3),
                        SelectedDate.AddDays(-3)
                    }.Where(d => d >= DateTime.Today).ToList();

                    foreach (var date in nearbyDates)
                    {
                        var venues = await _venueReservationService.GetAvailableVenues(date, SelectedEventTypeId);
                        if (venues.Any())
                        {
                            AvailableVenues = venues;
                            SelectedDate = date;
                            Event.EventDate = date;
                            Message = $"No venues available for your original date. Found venues for {date:MMMM dd, yyyy} instead.";
                            break;
                        }
                    }
                }

                // If still no venues, show helpful message
                if (!AvailableVenues.Any())
                {
                    Message = $"No venues available for {SelectedEventTypeId} on {SelectedDate:MMMM dd, yyyy}. " +
                             "This could be because:\n" +
                             "1. The venue is already booked for another event\n" +
                             "2. No suitable venues for this event type\n" +
                             "3. Date is outside available range\n\n" +
                             "Try dates 2-3 months from now, or check different dates.";
                }

                _logger.LogInformation($"Found {AvailableVenues.Count} venues for {SelectedEventTypeId} on {SelectedDate:yyyy-MM-dd}");

                // Update Event object
                Event.EventDate = SelectedDate;
                Event.EventTypeId = SelectedEventTypeId;

                // Update select lists
                EventTypeSelectList = new SelectList(EventTypes, "Id", "Title", SelectedEventTypeId);
                VenueSelectList = new SelectList(AvailableVenues, "Code", "Name");

                HasCheckedVenues = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading venues");
                Message = "Error checking available venues. Please try again.";
                await LoadEventTypes();
                GenerateSuggestedDates();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadEventTypes();
                    GenerateSuggestedDates();
                    return Page();
                }

                // Set event type and date
                Event.EventTypeId = SelectedEventTypeId;
                Event.EventDate = SelectedDate;

                // DOUBLE-BOOKING PREVENTION: Check venue availability again
                var currentAvailability = await _venueReservationService.GetAvailableVenues(
                    Event.EventDate,
                    Event.EventTypeId
                );

                var isVenueStillAvailable = currentAvailability.Any(v => v.Code == SelectedVenueCode);

                if (!isVenueStillAvailable)
                {
                    Message = $"⚠️ Venue {SelectedVenueCode} is no longer available for {Event.EventDate:MMMM dd, yyyy}. " +
                             "It may have been booked by someone else while you were filling the form.\n\n" +
                             "Please select a different venue or date.";

                    // Reload available venues
                    AvailableVenues = currentAvailability;
                    VenueSelectList = new SelectList(AvailableVenues, "Code", "Name");
                    HasCheckedVenues = true;

                    await LoadEventTypes();
                    GenerateSuggestedDates();
                    return Page();
                }

                // Make reservation if venue selected
                if (!string.IsNullOrEmpty(SelectedVenueCode))
                {
                    _logger.LogInformation($"Reserving venue {SelectedVenueCode} for {Event.EventDate:yyyy-MM-dd}");

                    var reservationReference = await _venueReservationService.ReserveVenue(
                        Event.EventDate,
                        SelectedVenueCode
                    );

                    if (reservationReference == null)
                    {
                        Message = "❌ Failed to reserve venue. Possible reasons:\n" +
                                 "1. Venue was just booked by someone else\n" +
                                 "2. Network error connecting to Apex.Venues\n" +
                                 "3. Venue is no longer available\n\n" +
                                 "Please try a different venue or check the Test Venues page.";

                        // Refresh available venues
                        AvailableVenues = await _venueReservationService.GetAvailableVenues(
                            Event.EventDate,
                            Event.EventTypeId
                        );
                        VenueSelectList = new SelectList(AvailableVenues, "Code", "Name");
                        HasCheckedVenues = true;

                        await LoadEventTypes();
                        GenerateSuggestedDates();
                        return Page();
                    }

                    Event.ReservationReference = reservationReference;
                    Event.VenueCode = SelectedVenueCode;

                    _logger.LogInformation($"Successfully reserved venue. Reference: {reservationReference}");
                }

                // Save event to database
                _context.Events.Add(Event);
                await _context.SaveChangesAsync();

                // Success message
                TempData["SuccessMessage"] = $"Event '{Event.EventName}' created successfully!";
                if (!string.IsNullOrEmpty(Event.ReservationReference))
                {
                    TempData["SuccessMessage"] += $" Reservation Reference: {Event.ReservationReference}";
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                Message = $"❌ Error creating event: {ex.Message}\n\nPlease try again or check the Test Venues page.";
                await LoadEventTypes();
                GenerateSuggestedDates();
                return Page();
            }
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event types");
                EventTypes = GetFallbackEventTypes();
            }
        }

        private void GenerateSuggestedDates()
        {
            SuggestedDates.Clear();

            // Generate 10 suggested dates starting 2 months from now
            var baseDate = DateTime.Today.AddMonths(2);

            for (int i = 0; i < 10; i++)
            {
                var date = baseDate.AddDays(i);
                SuggestedDates.Add(date);
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