using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apex.Catering.Data;
using Apex.Catering.Dto;

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

        // GET all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodBookingDto>>> GetFoodBookings()
        {
            return await _context.FoodBookings
                .Select(f => new FoodBookingDto
                {
                    ClintReferenceId = f.ClintReferenceId,
                    MenuId = f.MenuId ?? 0,
                    NumberOfGuests = f.NumberOfGuests ?? 0
                })
                .ToListAsync();
        }

        // GET single booking
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodBookingDto>> GetFoodBooking(int id)
        {
            var f = await _context.FoodBookings.FindAsync(id);
            if (f == null) return NotFound();

            return new FoodBookingDto
            {
                ClintReferenceId = f.ClintReferenceId,
                MenuId = f.MenuId ?? 0,
                NumberOfGuests = f.NumberOfGuests ?? 0
            };
        }

        // POST new booking
        [HttpPost]
        public async Task<ActionResult<FoodBookingDto>> PostFoodBooking(FoodBookingDto dto)
        {
            var booking = new FoodBooking
            {
                ClintReferenceId = dto.ClintReferenceId,
                MenuId = dto.MenuId,
                NumberOfGuests = dto.NumberOfGuests
            };

            _context.FoodBookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFoodBooking), new { id = booking.FoodBookingId }, dto);
        }

        // PUT update booking
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodBooking(int id, FoodBookingDto dto)
        {
            var booking = await _context.FoodBookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.ClintReferenceId = dto.ClintReferenceId;
            booking.MenuId = dto.MenuId;
            booking.NumberOfGuests = dto.NumberOfGuests;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodBooking(int id)
        {
            var booking = await _context.FoodBookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.FoodBookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
//         public async Task<IActionResult> DeleteMenu(int id)