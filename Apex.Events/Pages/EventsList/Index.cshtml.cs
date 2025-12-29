using System.Collections.Generic;
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

        public async Task OnGetAsync()
        {
            Events = await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }
    }
}
