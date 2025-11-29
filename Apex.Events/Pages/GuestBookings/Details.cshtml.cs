using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Events.Pages.GuestBookings
{
    public class DetailsModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DetailsModel(EventsDbContext context)
        {
            _context = context;
        }

        public GuestBooking GuestBooking { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            GuestBooking = await _context.GuestBookings
                .Include(g => g.Guest)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(g => g.GuestBookingId == id);

            if (GuestBooking == null)
                return NotFound();

            return Page();
        }
    }
}
