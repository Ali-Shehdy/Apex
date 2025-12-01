using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Events.Pages.GuestsCount
{
    public class IndexModel : PageModel
    {
        private readonly EventsDbContext _context;

        public IndexModel(EventsDbContext context)
        {
            _context = context;
        }

        public IList<Event> EventList { get; set; }

        public async Task OnGetAsync()
        {
            EventList = await _context.Events
                .Include(e => e.GuestBookings)
                .ToListAsync();
        }
    }
}
