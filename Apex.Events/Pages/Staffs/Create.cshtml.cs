using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Apex.Events.Data;

namespace Apex.Events.Pages.Staffs
{
    [Authorize(Roles = "Manager")]
    public class CreateModel : PageModel
    {
        private readonly EventsDbContext _context;

        public CreateModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Staff Staff { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Staffs.Add(Staff);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
