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

namespace Helpdesk.Pages.SupervisorResponsibilities
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
      public SupervisorResponsibility SupervisorResponsibility { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SuperRespsAdminAccess);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.SupervisorResponsibilities == null)
            {
                return NotFound();
            }

            var supervisorresponsibility = await _context.SupervisorResponsibilities.FirstOrDefaultAsync(m => m.Id == id);

            if (supervisorresponsibility == null)
            {
                return NotFound();
            }
            else 
            {
                SupervisorResponsibility = supervisorresponsibility;
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SuperRespsAdminAccess);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.SupervisorResponsibilities == null)
            {
                return NotFound();
            }
            var supervisorresponsibility = await _context.SupervisorResponsibilities.FindAsync(id);

            if (supervisorresponsibility != null)
            {
                SupervisorResponsibility = supervisorresponsibility;
                _context.SupervisorResponsibilities.Remove(SupervisorResponsibility);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
