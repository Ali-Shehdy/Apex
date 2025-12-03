using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.Staffs
{
    public class DetailsModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DetailsModel(EventsDbContext context)
        {
            _context = context;
        }

        public Staff Staff { get; set; } = default!;
        public List<Event> UpcomingEvents { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Staff = await _context.Staffs
                .Include(s => s.Staffings)
                    .ThenInclude(st => st.Event)
                .FirstOrDefaultAsync(s => s.StaffId == id);

            if (Staff == null) return NotFound();

            UpcomingEvents = Staff.Staffings
                .Where(st => st.Event.EventDate >= DateTime.Today)
                .Select(st => st.Event)
                .OrderBy(e => e.EventDate)
                .ToList();

            return Page();
        }
    }
}
