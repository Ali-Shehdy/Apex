using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Events.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Events.Pages.GuestBookings
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;

        public CreateModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GuestBooking GuestBooking { get; set; } = new GuestBooking();

        public SelectList GuestList { get; set; }
        public SelectList EventList { get; set; }

        public IActionResult OnGet()
        {
            GuestList = new SelectList(_context.Guests, "GuestId", "FirstName");
            EventList = new SelectList(_context.Events, "EventId", "EventName");

            // Default booking date = today
            GuestBooking.BookingDate = DateTime.Today;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
           // // Validate
           // //if (!ModelState.IsValid)
           // //{
           //     GuestList = new SelectList(_context.Guests, "GuestId", "FirstName");
           //     EventList = new SelectList(_context.Events, "EventId", "EventName");
           //     return Page();
           //// }

            _context.GuestBookings.Add(GuestBooking);
            await _context.SaveChangesAsync();
            Console.WriteLine("SAVED: " + GuestBooking.GuestBookingId);

            await _context.SaveChangesAsync();
            Console.WriteLine("NEW ID = " + GuestBooking.GuestBookingId);


            return RedirectToPage("./Index"); // Go back to list
        }
    }
}
