using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Events.Pages.GuestBookings
{
    public class IndexModel : PageModel
    {
        private readonly EventsDbContext _context;

        public IndexModel(EventsDbContext context)
        {
            _context = context;
        }

        public IList<GuestBooking> GuestBookingList { get; set; }

        public async Task OnGetAsync()
        {
            GuestBookingList = await _context.GuestBookings
                .Include(g => g.Guest)
                .Include(e => e.Event)
                .ToListAsync();
        }
    }
}
