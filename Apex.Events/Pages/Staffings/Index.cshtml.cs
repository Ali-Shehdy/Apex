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
        public ISet<int> EventsWithStaffingWarning { get; private set; } = new HashSet<int>();

        public async Task OnGetAsync()
        {
            Events = await _context.Events
                       .Include(e => e.Staffings)
                       .Include(e => e.GuestBookings)
                       .AsNoTracking()
                       .ToListAsync();

            EventsWithStaffingWarning = Events
                .Where(e => HasStaffingWarning(e.GuestBookings.Count, e.Staffings.Count))
                .Select(e => e.EventId)
                .ToHashSet();
        }

        private static bool HasStaffingWarning(int guestCount, int staffCount)
        {
            if (guestCount <= 0)
            {
                return false;
            }

            var requiredStaff = (int)Math.Ceiling(guestCount / 10.0);
            return staffCount < requiredStaff;
        }
    }
}
