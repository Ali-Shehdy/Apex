using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data; 
using Apex.Events.Dto; 

namespace Apex.Events.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public EventsController(EventsDbContext context)
        {
            _context = context;
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
                EventType = dto.EventType,
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
            existingEvent.EventType = dto.EventType;

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

            _context.Events.Remove(evnt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}