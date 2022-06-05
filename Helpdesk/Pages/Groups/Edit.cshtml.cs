using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.Groups
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public Group Group { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var group =  await _context.Groups.FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            Group = group;
            return Page();
        }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existGroup = await _context.Groups.Where(x => x.Id != Group.Id && x.Name == Group.Name).FirstOrDefaultAsync();
            if (existGroup != null)
            {
                ModelState.AddModelError("Group.Name", "This group already exists.");
                return Page();
            }
            existGroup = await _context.Groups.Where(x => x.Id == Group.Id).FirstOrDefaultAsync();

            if (existGroup == null)
            {
                return NotFound();
            }

            existGroup.Name = Group.Name;
            existGroup.Description = Group.Description;

            _context.Groups.Update(existGroup);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(Group.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool GroupExists(int id)
        {
          return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
