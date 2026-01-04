using System.ComponentModel.DataAnnotations;
using Apex.Events.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Apex.Events.Pages.Account
{
    [Authorize(Roles = "Manager")]
    public class RegisterModel : PageModel
    {
        private readonly EventsDbContext _context;

        public RegisterModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var emailExists = await _context.Staffs.AnyAsync(s => s.Email == Input.Email);
            if (emailExists)
            {
                ModelState.AddModelError(string.Empty, "A staff member with this email already exists.");
                return Page();
            }

            var staff = new Staff
            {
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim(),
                Email = Input.Email.Trim(),
                Phone = Input.Phone.Trim(),
                Role = Input.Role.Trim()
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Staffs/Index");
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string Phone { get; set; } = string.Empty;

            [Required]
            public string Role { get; set; } = string.Empty;
        }
    }
}