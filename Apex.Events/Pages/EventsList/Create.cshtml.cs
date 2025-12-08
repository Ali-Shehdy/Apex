using Apex.Events.Data;
using Apex.Events.Models;
using Apex.Events.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateModel : PageModel
{
    private readonly EventTypeService _eventTypeService;
    private readonly EventsDbContext _context;

    public List<EventTypeDTO> EventTypes { get; set; } = new();

    public CreateModel(EventsDbContext context, EventTypeService eventTypeService)
    {
        _context = context;
        _eventTypeService = eventTypeService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        EventTypes = await _eventTypeService.GetEventTypesAsync();
        return Page();
    }

    [BindProperty]
    public Apex.Events.Data.Event Event { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            EventTypes = await _eventTypeService.GetEventTypesAsync();
            return Page();
        }

        _context.Events.Add(Event);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
