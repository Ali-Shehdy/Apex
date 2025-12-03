using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apex.Events.Pages.Staffings
{
    public class IndexModel : PageModel
    {
        private readonly EventsDbContext _context;

        public IndexModel(EventsDbContext context)
        {
            _context = context;
        }

        public IList<Event> Events { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Events = await _context.Events.ToListAsync();
        }
    }
}
