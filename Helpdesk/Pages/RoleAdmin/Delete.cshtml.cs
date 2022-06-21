using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.RoleAdmin
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
      public HelpdeskRole HelpdeskRole { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            if (id == null || _context.HelpdeskRoles == null)
            {
                return NotFound();
            }

            var helpdeskrole = await _context.HelpdeskRoles.FirstOrDefaultAsync(m => m.Id == id);

            if (helpdeskrole == null)
            {
                return NotFound();
            }
            else if (helpdeskrole.IsSuperAdmin)
            {
                ModelState.AddModelError("", "You can't delete a Super Admin role.");
            }
            else 
            {
                HelpdeskRole = helpdeskrole;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
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
            if (id == null || _context.HelpdeskRoles == null)
            {
                return NotFound();
            }
            var helpdeskrole = await _context.HelpdeskRoles.FindAsync(id);

            if (helpdeskrole != null)
            {
                if (helpdeskrole.IsSuperAdmin)
                {
                    ModelState.AddModelError("", "You can't delete a Super Admin role.");
                    return Page();
                }
                HelpdeskRole = helpdeskrole;
                _context.HelpdeskRoles.Remove(HelpdeskRole);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
