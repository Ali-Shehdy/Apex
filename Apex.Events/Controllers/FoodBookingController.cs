using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Catering.Data;

namespace Apex.Events.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodBookingController : ControllerBase
    {
        private readonly CateringDbContext _context;

        public FoodBookingController(CateringDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodBooking
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.FoodBookings
                .Include(b => b.Menu)
                .ToListAsync();
            return Ok(bookings);
        }

        // GET: api/FoodBooking/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.FoodBookings
                .Include(b => b.Menu)
                .FirstOrDefaultAsync(b => b.FoodBookingId == id);

            if (booking == null) return NotFound();
            return Ok(booking);
        }

        // POST: api/FoodBooking
        [HttpPost]
        public async Task<IActionResult> CreateBooking(FoodBooking booking)
        {
            _context.FoodBookings.Add(booking);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooking), new { id = booking.FoodBookingId }, booking);
        }

        // PUT: api/FoodBooking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, FoodBooking updated)
        {
            if (id != updated.FoodBookingId) return BadRequest("ID mismatch");

            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/FoodBooking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.FoodBookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.FoodBookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
