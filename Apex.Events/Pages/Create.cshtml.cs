using Apex.Events.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
namespace Apex.Events.Pages
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;
        public CreateModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Apex.Events.Data.Event EventItem { get; set; } = new Apex.Events.Data.Event();

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Events.Add(EventItem);
            _context.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}