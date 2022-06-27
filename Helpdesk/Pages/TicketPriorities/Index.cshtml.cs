using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Infrastructure;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.TicketPriorities
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public IList<TicketPriority> TicketPriority { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (_context.TicketPriority != null)
            {
                TicketPriority = await _context.TicketPriority.ToListAsync();
            }
            return Page();
        }
    }
}
