using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Apex.Events.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Apex.Events.Pages.Staffings
{
    public class DeleteModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DeleteModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Staffing Staffing { get; set; } = default!;

        public Staff Staff { get; set; } = default!;
        public Event Event { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Staffing = await _context.Staffings
                .Include(s => s.Staff)
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.StaffingId == id);

            if (Staffing == null) return NotFound();

            Staff = Staffing.Staff;
            Event = Staffing.Event;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var staffing = await _context.Staffings.FindAsync(id);
            if (staffing != null)
            {
                _context.Staffings.Remove(staffing);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
