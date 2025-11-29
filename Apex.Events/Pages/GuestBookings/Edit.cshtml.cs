using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;
using System.Threading.Tasks;

namespace Apex.Events.Pages.GuestBookings
{
    public class EditModel : PageModel
    {
        private readonly EventsDbContext _context;

        public EditModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuestBooking GuestBooking { get; set; }

        public SelectList GuestList { get; set; }
        public SelectList EventList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            GuestBooking = await _context.GuestBookings.FindAsync(id);

            if (GuestBooking == null)
                return NotFound();

            GuestList = new SelectList(_context.Guests, "GuestId", "FirstName");
            EventList = new SelectList(_context.Events, "EventId", "EventName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(GuestBooking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
