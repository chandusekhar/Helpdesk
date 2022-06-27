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

namespace Helpdesk.Pages.TicketPriorities
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
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

            var ticketpriority =  await _context.TicketPriority.FirstOrDefaultAsync(m => m.Id == id);
            if (ticketpriority == null)
            {
                return NotFound();
            }
            TicketPriority = ticketpriority;
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

            var tp = await _context.TicketPriority.Where(x => x.Id == TicketPriority.Id).FirstOrDefaultAsync();
            if (tp == null)
            {
                return NotFound();
            }
            if (tp.Name != TicketPriority.Name)
            {
                var tpe = await _context.TicketPriority.Where(x => x.Name == TicketPriority.Name).FirstOrDefaultAsync();
                if (tpe != null)
                {
                    ModelState.AddModelError("TicketPriority.Name", "Name is already in use.");
                    return Page();
                }
            }
            tp.Name = TicketPriority.Name;
            tp.Description = TicketPriority.Description;
            _context.TicketPriority.Update(tp);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private bool TicketPriorityExists(int id)
        {
          return (_context.TicketPriority?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
