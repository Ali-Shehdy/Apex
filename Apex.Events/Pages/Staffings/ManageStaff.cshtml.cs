using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace Apex.Events.Pages.Staffings
{
    [Authorize(Roles = "Manager,TeamLeader")]
    public class ManageStaffModel : PageModel
    {
        private readonly EventsDbContext _context;

        public ManageStaffModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        [BindProperty]
        public int SelectedStaffId { get; set; }

        public IList<Staffing> Staffings { get; set; } = new List<Staffing>();
        public List<Staff> AvailableStaff { get; set; } = new List<Staff>();
        public SelectList StaffList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? eventId)
        {
            if (eventId == null) return NotFound();

            Event = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (Event == null) return NotFound();

            Staffings = await _context.Staffings
                .Where(s => s.EventId == eventId)
                .Include(s => s.Staff)
                .ToListAsync();

            var assignedIds = Staffings.Select(s => s.StaffId).ToList();
            AvailableStaff = await _context.Staffs
                .Where(s => !assignedIds.Contains(s.StaffId))
                .OrderBy(s => s.LastName)
                .ToListAsync();

            // prepare SelectList for the dropdown (value = StaffId, text = "First Last (Role)")
            StaffList = new SelectList(
                AvailableStaff.Select(s => new {
                    s.StaffId,
                    Display = $"{s.FirstName} {s.LastName} ({s.Role})"
                }),
                "StaffId",
                "Display"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAddStaffAsync(int eventId)
        {
            if (SelectedStaffId == 0) return RedirectToPage(new { eventId });

            // check existence and duplicates
            var staffExists = await _context.Staffs.AnyAsync(s => s.StaffId == SelectedStaffId);
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            var already = await _context.Staffings.AnyAsync(s => s.EventId == eventId && s.StaffId == SelectedStaffId);

            if (!staffExists || !eventExists || already)
                return RedirectToPage(new { eventId });

            var staffing = new Staffing { EventId = eventId, StaffId = SelectedStaffId };
            _context.Staffings.Add(staffing);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { eventId });
        }

        public async Task<IActionResult> OnPostRemoveStaffAsync(int eventId, int staffingId)
        {
            var staffing = await _context.Staffings.FindAsync(staffingId);
            if (staffing != null)
            {
                _context.Staffings.Remove(staffing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { eventId });
        }
    }
}
