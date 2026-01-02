using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.EventsList
{
    public class IndexModel : PageModel
    {
        private readonly EventsDbContext _context;

        public IndexModel(EventsDbContext context)
        {
            _context = context;
        }

        public IList<Event> Events { get; private set; } = new List<Event>();
        public IList<Event> UpcomingEventsWithoutFirstAider { get; private set; } = new List<Event>();

        public async Task OnGetAsync()
        {
            var events = await _context.Events
                .Include(e => e.Staffings)
                .ThenInclude(s => s.Staff)
                .AsNoTracking()
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            Events = events;

            var today = DateTime.Today;
            UpcomingEventsWithoutFirstAider = events
                .Where(e => e.EventDate.Date >= today)
                .Where(e => !e.Staffings.Any())
                .OrderBy(e => e.EventDate)
                .ToList();
        }
    }
}
