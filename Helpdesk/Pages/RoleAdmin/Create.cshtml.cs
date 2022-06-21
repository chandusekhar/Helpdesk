using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.RoleAdmin
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            [Display(Name = "Privileged Role")]
            public bool IsPrivileged { get; set; }
            [Required]
            [Display(Name = "Super Admin")]
            public bool IsSuperAdmin { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.HelpdeskRolesAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            return Page();
        }

        [BindProperty]
        public InputModel HelpdeskRole { get; set; } = default!;
        
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.HelpdeskRolesAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid || _context.HelpdeskRoles == null || HelpdeskRole == null)
            {
                return Page();
            }

            var role = await _context.HelpdeskRoles.Where(x => x.Name == HelpdeskRole.Name).FirstOrDefaultAsync();
            if (role != null)
            {
                ModelState.AddModelError("HelpdeskRole.Name", "Role name is already in use.");
                return Page();
            }
            role = new HelpdeskRole()
            {
                Name = HelpdeskRole.Name,
                Description = HelpdeskRole.Description,
                IsPrivileged = HelpdeskRole.IsPrivileged,
                IsSuperAdmin = HelpdeskRole.IsSuperAdmin
            };
            _context.HelpdeskRoles.Add(role);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = role.Id });
        }
    }
}
