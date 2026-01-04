using Apex.Events.Data; 
using Apex.Events.Dto; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Services;

namespace Apex.Events.Controllers
{
    [ApiController] // Indicates that this class is an API controller
    [Route("api/[controller]")] // Sets the route for the API endpoints
    public class EventsController : ControllerBase // Inherits from ControllerBase for API functionality
    {
        private readonly EventsDbContext _context; // Database context for accessing event data
        private readonly IVenueReservationService _venueReservationService; // Service for managing venue reservations

        // Constructor with dependency injection
        public EventsController(EventsDbContext context, IVenueReservationService venueReservationService) 
        {
            _context = context;    // Initialize the database context
            _venueReservationService = venueReservationService; // Initialize the venue reservation service
        }

        // POST: api/events
        [HttpPost] // Endpoint to create a new event
        public async Task<ActionResult<Event>> CreateEvent([FromBody]CreateEventDto dto) // Accepts event data in the request body
        {
            if (dto == null)
            {
                return BadRequest("Event data is invalid."); // Return 400 Bad Request if the DTO is null
            }

            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
            };

            _context.Events.Add(newEvent); // Add the new event to the database context
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        [HttpGet("{id}")] // GET: api/events/{id}
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var evnt = await _context.Events
                .Include(e => e.GuestBookings)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (evnt == null)
                return NotFound();

            return evnt;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var eventsList = await _context.Events
                .Include(e => e.GuestBookings)
                .ToListAsync();

            return Ok(eventsList);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto dto)
        {
            if (id != dto.EventId || dto == null) // Validate the event ID and DTO
            {
                return BadRequest("Event data is invalid.");
            }

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.EventName = dto.EventName; // Update event name
            existingEvent.EventDate = dto.EventDate; // Update event date

            _context.Entry(existingEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id) // Soft delete an event
        {
            var evnt = await _context.Events.FindAsync(id);
            if (evnt == null)
            {
                return NotFound();
            }

            var staffings = await _context.Staffings // Remove associated staffings
                         .Where(s => s.EventId == id)
                         .ToListAsync();

            if (staffings.Count > 0)
            {
                _context.Staffings.RemoveRange(staffings);
            }

            if (!string.IsNullOrWhiteSpace(evnt.ReservationReference))
            {
                await _venueReservationService.FreeReservation(evnt.ReservationReference);
            }

            evnt.ReservationReference = null;
            evnt.VenueCode = null;
            evnt.IsCancelled = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id) // Check if an event exists by ID
        {
            return _context.Events.Any(e => e.EventId == id); // Return true if the event exists, otherwise false
        }
    }
}