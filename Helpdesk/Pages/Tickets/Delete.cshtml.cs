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

namespace Helpdesk.Pages.Tickets
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public TicketMaster TicketMaster { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasSubmitter = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketSubmitter);
            bool HasHandler = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketHandler);
            if (!HasSubmitter && !HasHandler)
            {
                return Forbid();
            }
            if (id == null || _context.TicketMasters == null)
            {
                return NotFound();
            }

            var ticketmaster = await _context.TicketMasters.FirstOrDefaultAsync(m => m.Id == id);

            if (ticketmaster == null)
            {
                return NotFound();
            }
            else 
            {
                TicketMaster = ticketmaster;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasSubmitter = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketSubmitter);
            bool HasHandler = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketHandler);
            if (!HasSubmitter && !HasHandler)
            {
                return Forbid();
            }
            if (id == null || _context.TicketMasters == null)
            {
                return NotFound();
            }
            var ticketmaster = await _context.TicketMasters.FindAsync(id);

            if (ticketmaster != null)
            {
                TicketMaster = ticketmaster;
                _context.TicketMasters.Remove(TicketMaster);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
