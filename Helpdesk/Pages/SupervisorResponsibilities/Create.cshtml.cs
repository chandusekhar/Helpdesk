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

namespace Helpdesk.Pages.SupervisorResponsibilities
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public async Task<IActionResult> OnGet()
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
            return Page();
        }

        [BindProperty]
        public SupervisorResponsibility SupervisorResponsibility { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
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
            if (!ModelState.IsValid || _context.SupervisorResponsibilities == null || SupervisorResponsibility == null)
            {
                return Page();
            }

            _context.SupervisorResponsibilities.Add(SupervisorResponsibility);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
