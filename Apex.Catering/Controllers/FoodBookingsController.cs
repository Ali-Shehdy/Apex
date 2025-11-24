using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Catering.Data;

namespace Apex.Catering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodBookingsController : ControllerBase
    {
        private readonly CateringDbContext _context;

        public FoodBookingsController(CateringDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodBookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodBooking>>> GetFoodBookings()
        {
            return await _context.FoodBookings.ToListAsync();
        }

        // GET: api/FoodBookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodBooking>> GetFoodBooking(int id)
        {
            var foodBooking = await _context.FoodBookings.FindAsync(id);

            if (foodBooking == null)
            {
                return NotFound();
            }

            return foodBooking;
        }

        [HttpPost]
        public async Task<ActionResult<FoodBooking>> PostFoodBooking(FoodBookingDto foodBooking)
        {
            bool menuExist = this.MenuExists(foodBooking);
            if (menuExist && foodBooking.clinetReferenceId != null && foodBooking.numberOfGuests > 0)
            {
                if (_context.FoodBookings.Any(m => m.ClintReferenceId == foodBooking.clinetReferenceId && m.MenuId == foodBooking.menuId))
                {
                    return Conflict(new { message = $"The Clint with {foodBooking.clinetReferenceId}  has an existing booking for menu {foodBooking.menuId}." });
                }
                FoodBooking newBooking = new FoodBooking
                {
                    ClintReferenceId = (int)foodBooking.clinetReferenceId,
                    MenuId = foodBooking.menuId,
                    NumberOfGuests = foodBooking.numberOfGuests,
                };
                _context.FoodBookings.Add(newBooking);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetFoodBooking", new { id = newBooking.FoodBookingId });
            }
            else
            {
                return BadRequest();
            }

        }


        // PUT: api/FoodBookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodBooking(int id, FoodBooking foodBooking)
        {
            if (id != foodBooking.FoodBookingId)
            {
                return BadRequest();
            }

            _context.Entry(foodBooking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodBookingExists(id))
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

        // POST: api/FoodBookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FoodBooking>> PostFoodBooking(FoodBooking foodBooking)
        {
            _context.FoodBookings.Add(foodBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFoodBooking", new { id = foodBooking.FoodBookingId }, foodBooking);
        }

        // DELETE: api/FoodBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodBooking(int id)
        {
            var foodBooking = await _context.FoodBookings.FindAsync(id);
            if (foodBooking == null)
            {
                return NotFound();
            }

            _context.FoodBookings.Remove(foodBooking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodBookingExists(int id)
        {
            return _context.FoodBookings.Any(e => e.FoodBookingId == id);
        }

        // Add this private method to the FoodBookingsController class to fix CS1061
        private bool MenuExists(FoodBookingDto foodBooking)
        {
            // Replace with actual logic to check if the menu exists.
            // For example, if foodBooking.menuId is the menu identifier:
            return _context.Menus.Any(m => m.MenuId == foodBooking.menuId);
        }
    }
}
