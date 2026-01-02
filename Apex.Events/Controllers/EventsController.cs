using Apex.Events.Data; 
using Apex.Events.Dto; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Services;

namespace Apex.Events.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventsDbContext _context;
        private readonly IVenueReservationService _venueReservationService;

        public EventsController(EventsDbContext context, IVenueReservationService venueReservationService)
        {
            _context = context;
            _venueReservationService = venueReservationService;
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody]CreateEventDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Event data is invalid.");
            }

            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
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
            if (id != dto.EventId || dto == null)
            {
                return BadRequest("Event data is invalid.");
            }

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.EventName = dto.EventName;
            existingEvent.EventDate = dto.EventDate;

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
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evnt = await _context.Events.FindAsync(id);
            if (evnt == null)
            {
                return NotFound();
            }

            var staffings = await _context.Staffings
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

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}