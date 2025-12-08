using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Apex.Events.Data;
using Apex.Events.Models;
using Apex.Events.Services;

namespace Apex.Events.EventsList
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;
        private readonly EventTypeService _eventTypeService;

        public CreateModel(EventsDbContext context, EventTypeService eventTypeService)
        {
            _context = context;
            _eventTypeService = eventTypeService;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        public List<EventTypeDTO> EventTypes { get; set; } = new List<EventTypeDTO>();

        public async Task<IActionResult> OnGetAsync()
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();
            if (EventTypes.Count == 0)
            {
                ModelState.AddModelError("", "Could not load Event Types from the Venues API.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
