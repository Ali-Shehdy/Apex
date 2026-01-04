using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Apex.Events.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Apex.Events.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly EventsDbContext _context;

        public LoginModel(EventsDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var staff = await _context.Staffs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == Input.Email && s.Phone == Input.Phone);

            if (staff == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or phone.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
                new Claim(ClaimTypes.Name, $"{staff.FirstName} {staff.LastName}".Trim()),
                new Claim(ClaimTypes.Email, staff.Email)
            };

            if (!string.IsNullOrWhiteSpace(staff.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, staff.Role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Phone]
            [Display(Name = "Phone (Password)")]
            public string Phone { get; set; } = string.Empty;
        }
    }
}