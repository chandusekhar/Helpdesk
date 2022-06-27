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

namespace Helpdesk.Pages.TicketPriorities
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public TicketPriority TicketPriority { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
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
            if (id == null || _context.TicketPriority == null)
            {
                return NotFound();
            }

            var ticketpriority = await _context.TicketPriority.FirstOrDefaultAsync(m => m.Id == id);

            if (ticketpriority == null)
            {
                return NotFound();
            }
            else if (ticketpriority.IsSystemType)
            {
                return Forbid();
            }
            bool used = await _context.TicketMasters.Where(x => x.TicketPriority == ticketpriority).AnyAsync();
            if (used)
            {
                ModelState.AddModelError("", "This priority is in use and cannot be deleted.");
            }
            TicketPriority = ticketpriority;
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.TicketPriority == null)
            {
                return NotFound();
            }
            var ticketpriority = await _context.TicketPriority.FindAsync(id);

            if (ticketpriority != null)
            {
                if (ticketpriority.IsSystemType)
                {
                    return Forbid();
                }
                bool used = await _context.TicketMasters.Where(x => x.TicketPriority == ticketpriority).AnyAsync();
                if (used)
                {
                    ModelState.AddModelError("", "This priority is in use and cannot be deleted.");
                    return Page();
                }
                TicketPriority = ticketpriority;
                _context.TicketPriority.Remove(TicketPriority);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
