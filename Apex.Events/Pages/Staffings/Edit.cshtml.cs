using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Apex.Events.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Apex.Events.Pages.Staffings
{
    [Authorize(Roles = "Manager,TeamLeader")]
    public class EditModel : PageModel
    {
        private readonly EventsDbContext _context;

        public EditModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Staffing Staffing { get; set; } = default!;

        public SelectList EventList { get; set; } = default!;
        public SelectList StaffList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Staffing = await _context.Staffings.FindAsync(id);
            if (Staffing == null) return NotFound();

            EventList = new SelectList(await _context.Events.OrderBy(e => e.EventDate).ToListAsync(), "EventId", "EventName", Staffing.EventId);
            StaffList = new SelectList(await _context.Staffs.OrderBy(s => s.LastName).ToListAsync(), "StaffId", "FirstName", Staffing.StaffId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Staffing.StaffingId == 0) return NotFound();

            if (Staffing.EventId == 0 || Staffing.StaffId == 0)
            {
                ModelState.AddModelError(string.Empty, "Please choose both an event and a staff member.");
            }

            // prevent duplicate (different staffing with same event+staff)
            var duplicate = await _context.Staffings.AnyAsync(s =>
                s.EventId == Staffing.EventId && s.StaffId == Staffing.StaffId && s.StaffingId != Staffing.StaffingId);
            if (duplicate)
            {
                ModelState.AddModelError(string.Empty, "This staffing would duplicate an existing assignment.");
            }

            if (!ModelState.IsValid)
            {
                EventList = new SelectList(await _context.Events.OrderBy(e => e.EventDate).ToListAsync(), "EventId", "EventName", Staffing.EventId);
                StaffList = new SelectList(await _context.Staffs.OrderBy(s => s.LastName).ToListAsync(), "StaffId", "FirstName", Staffing.StaffId);
                return Page();
            }

            try
            {
                _context.Attach(Staffing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Staffings.AnyAsync(e => e.StaffingId == Staffing.StaffingId))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
