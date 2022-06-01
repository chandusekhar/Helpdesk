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

namespace Helpdesk.Pages.SiteSettings
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public IList<ConfigOpt> ConfigOpt { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await LoadSiteSettings(ViewData))
            {
                return RedirectToPage("/Index");
            }
            if (_currentHelpdeskUser == null)
            {
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SitewideConfigurationEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (_context.ConfigOpts != null)
            {
                ConfigOpt = await _context.ConfigOpts.OrderBy(x => x.Category).ThenBy(y => y.Order).ToListAsync();
            }

            return Page();
        }
    }
}
