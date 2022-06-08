using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Pages.ImportExport
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(ApplicationDbContext dbContext,
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.ImportExport);
            if (!HasClaim)
            {
                return Forbid();
            }

            return Page();
        
        }
    }
}
