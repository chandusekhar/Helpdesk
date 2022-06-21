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

namespace Helpdesk.Pages.TicketStatuses
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public TicketStatus TicketStatus { get; set; } = default!;

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
            if (id == null || _context.TicketStatuses == null)
            {
                return NotFound();
            }

            var ticketstatus =  await _context.TicketStatuses.FirstOrDefaultAsync(m => m.Id == id);
            if (ticketstatus == null)
            {
                return NotFound();
            }
            TicketStatus = ticketstatus;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var ts = await _context.TicketStatuses.Where(x => x.Id == TicketStatus.Id).FirstOrDefaultAsync();
            if (ts == null)
            {
                return NotFound();
            }
            if (ts.Name != TicketStatus.Name)
            {
                var tse = await _context.TicketStatuses.Where(x => x.Name == TicketStatus.Name).FirstOrDefaultAsync();
                if (tse != null)
                {
                    ModelState.AddModelError("TicketStatus.Name", "Status name is already in use.");
                    return Page();
                }
            }
            ts.Name = TicketStatus.Name;
            ts.Description = TicketStatus.Description;
            ts.IsCompleted = TicketStatus.IsCompleted;
            ts.Archived = TicketStatus.Archived;

            _context.TicketStatuses.Update(ts);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool TicketStatusExists(int id)
        {
          return (_context.TicketStatuses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
