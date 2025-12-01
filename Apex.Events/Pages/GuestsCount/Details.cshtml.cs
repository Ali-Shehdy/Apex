using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Threading.Tasks;

namespace Apex.Events.Pages.GuestsCount
{
    public class DetailsModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DetailsModel(EventsDbContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Event = await _context.Events
                .Include(e => e.GuestBookings)
                    .ThenInclude(g => g.Guest)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (Event == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
