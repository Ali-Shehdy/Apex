using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Apex.Events.Data;

namespace Apex.Events.Pages.GuestsList
{
    public class IndexModel : PageModel
    {
        private readonly Apex.Events.Data.EventsDbContext _context;

        public IndexModel(Apex.Events.Data.EventsDbContext context)
        {
            _context = context;
        }

        public IList<Guest> Guest { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Guest = await _context.Guests.ToListAsync();
        }
    }
}
