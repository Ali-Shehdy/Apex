using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.Staffs
{
    [Authorize(Roles = "Manager,TeamLeader")]
    public class DeleteModel : PageModel
    {
        private readonly EventsDbContext _context;

        public DeleteModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Staff Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == id);

            if (Staff == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _context.Staffs.FindAsync(id);

            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
