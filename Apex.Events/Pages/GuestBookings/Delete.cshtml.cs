using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Threading.Tasks;

namespace Apex.Events.Pages.GuestBookings
{
    public class DeleteModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DeleteModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuestBooking GuestBooking { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            GuestBooking = await _context.GuestBookings
                .Include(g => g.Guest)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(m => m.GuestBookingId == id);

            if (GuestBooking == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var booking = await _context.GuestBookings.FindAsync(id);

            if (booking != null)
            {
                _context.GuestBookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
