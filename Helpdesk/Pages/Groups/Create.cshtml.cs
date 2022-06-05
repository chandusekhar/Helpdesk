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

namespace Helpdesk.Pages.Groups
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
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.GroupAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            return Page();
        }

        [BindProperty]
        public Group Group { get; set; } = default!;
        
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.GroupAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid || _context.Groups == null || Group == null)
            {
                return Page();
            }
            var group = await _context.Groups.Where(x => x.Name == Group.Name).FirstOrDefaultAsync();
            if (group != null)
            {
                ModelState.AddModelError("Group.Name", "This group already exists.");
                return Page();
            }
            var newGroup = new Group()
            {
                Name = Group.Name,
                Description = Group.Description
            };
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
