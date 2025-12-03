using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.Staffs
{
    public class IndexModel : PageModel
    {
        private readonly EventsDbContext _context;

        public IndexModel(EventsDbContext context)
        {
            _context = context;
        }

        public IList<Staff> StaffList { get; set; } = new List<Staff>();

        public async Task OnGetAsync()
        {
            StaffList = await _context.Staffs.ToListAsync();
        }
    }
}
