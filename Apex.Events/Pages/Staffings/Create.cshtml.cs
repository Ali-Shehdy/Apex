using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Apex.Events.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Apex.Events.Pages.Staffings
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;

        public CreateModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Staffing Staffing { get; set; } = new();

        public SelectList EventList { get; set; } = default!;
        public SelectList StaffList { get; set; } = default!;

        // optionally preselect event
        public async Task<IActionResult> OnGetAsync(int? eventId)
        {
            EventList = new SelectList(await _context.Events.OrderBy(e => e.EventDate).ToListAsync(), "EventId", "EventName");
            StaffList = new SelectList(await _context.Staffs.OrderBy(s => s.LastName).ToListAsync(), "StaffId", "FirstName");

            if (eventId.HasValue)
                Staffing.EventId = eventId.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // basic validation
            if (Staffing.EventId == 0 || Staffing.StaffId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose both an event and a staff member.");
            }

            // prevent duplicate
            var exists = await _context.Staffings
                .AnyAsync(s => s.EventId == Staffing.EventId && s.StaffId == Staffing.StaffId);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This staff member is already assigned to that event.");
            }

            if (!ModelState.IsValid)
            {
                EventList = new SelectList(await _context.Events.OrderBy(e => e.EventDate).ToListAsync(), "EventId", "EventName");
                StaffList = new SelectList(await _context.Staffs.OrderBy(s => s.LastName).ToListAsync(), "StaffId", "FirstName");
                return Page();
            }

            // verify FK existence to avoid foreign key failures
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == Staffing.EventId);
            var staffExists = await _context.Staffs.AnyAsync(s => s.StaffId == Staffing.StaffId);
            if (!eventExists || !staffExists)
            {
                ModelState.AddModelError(string.Empty, "Selected Event or Staff no longer exists.");
                EventList = new SelectList(await _context.Events.OrderBy(e => e.EventDate).ToListAsync(), "EventId", "EventName");
                StaffList = new SelectList(await _context.Staffs.OrderBy(s => s.LastName).ToListAsync(), "StaffId", "FirstName");
                return Page();
            }

            _context.Staffings.Add(Staffing);
            await _context.SaveChangesAsync();

            // if created from ManageStaff context, redirect back there
            return RedirectToPage("Index");
        }
    }
}
