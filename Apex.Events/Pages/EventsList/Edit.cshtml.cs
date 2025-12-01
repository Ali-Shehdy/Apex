using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.EventsList
{
    public class EditModel : PageModel
    {
        private readonly Apex.Events.Data.EventsDbContext _context;

        public EditModel(Apex.Events.Data.EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Apex.Events.Data.Event Event { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events =  await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);
            if (events == null)
            {
                return NotFound();
            }
            Event = events;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Load the existing event to preserve non-editable fields
            var existingEvent = await _context.Events.AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == Event.EventId);

            if (existingEvent == null)
                return NotFound();

            // Preserve original values (date and type)
            Event.EventDate = existingEvent.EventDate;
            Event.EventType = existingEvent.EventType;

            // Update only editable fields
            _context.Attach(Event).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
