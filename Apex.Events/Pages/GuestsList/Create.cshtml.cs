using System.Threading.Tasks;
using Apex.Events.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Apex.Events.Pages.GuestsList
{
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;

        public CreateModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Guest Guest { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Guests.Add(Guest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Guest created successfully.";
            return RedirectToPage("./Index");
        }
    }
}
