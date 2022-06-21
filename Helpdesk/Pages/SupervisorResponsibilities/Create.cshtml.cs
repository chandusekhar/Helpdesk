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

            var exist = await _context.SupervisorResponsibilities.Where(x => x.Name == SupervisorResponsibility.Name).AnyAsync();
            if (exist)
            {
                ModelState.AddModelError("SupervisorResponsibility.Name", "The name is already in use.");
                return Page();
            }

            var sup = new SupervisorResponsibility()
            {
                Name = SupervisorResponsibility.Name,
                Description = SupervisorResponsibility.Description
            };

            _context.SupervisorResponsibilities.Add(sup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
