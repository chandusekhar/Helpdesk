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

namespace Helpdesk.Pages.ActionStatuses
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
      public ActionStatus ActionStatus { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.AssetOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.ActionStatuses == null)
            {
                return NotFound();
            }

            var actionstatus = await _context.ActionStatuses.FirstOrDefaultAsync(m => m.Id == id);

            if (actionstatus == null)
            {
                return NotFound();
            }
            else 
            {
                ActionStatus = actionstatus;
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.AssetOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.ActionStatuses == null)
            {
                return NotFound();
            }
            var actionstatus = await _context.ActionStatuses.FindAsync(id);

            if (actionstatus != null)
            {
                ActionStatus = actionstatus;
                _context.ActionStatuses.Remove(ActionStatus);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
