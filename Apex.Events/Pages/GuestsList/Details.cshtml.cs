using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.GuestsList
{
    public class DetailsModel : PageModel
    {
        private readonly Apex.Events.Data.EventsDbContext _context;

        public DetailsModel(Apex.Events.Data.EventsDbContext context)
        {
            _context = context;
        }

        public Guest Guest { get; set; } = default!;
        public List<GuestBooking> GuestBookings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestId == id);

            if (Guest == null)
                return NotFound();

            // Load guest bookings + event info
            GuestBookings = await _context.GuestBookings
                .Where(gb => gb.GuestId == id)
                .Include(gb => gb.Event)
                .ToListAsync();

            return Page();
        }
    }
}
