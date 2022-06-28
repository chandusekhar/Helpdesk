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

namespace Helpdesk.Pages.TicketTypes
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public TicketType TicketType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TicketTypes == null)
            {
                return NotFound();
            }

            var tickettype = await _context.TicketTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (tickettype == null)
            {
                return NotFound();
            }
            var inuse = await _context.TicketMasters.Where(x => x.TicketType == tickettype).AnyAsync();
            if (inuse)
            {
                ModelState.AddModelError("", "Ticket Type is in use, and can't be deleted.");
            }
            TicketType = tickettype;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.TicketTypes == null)
            {
                return NotFound();
            }
            var tickettype = await _context.TicketTypes.FindAsync(id);

            if (tickettype != null)
            {
                var inuse = await _context.TicketMasters.Where(x => x.TicketType == tickettype).AnyAsync();
                if (inuse)
                {
                    ModelState.AddModelError("", "Ticket Type is in use, and can't be deleted.");
                    return Page();
                }
                _context.TicketTypes.Remove(tickettype);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(tickettype.ViewClaim))
                {
                    var claim = await _context.HelpdeskClaims.Where(x => x.Name == tickettype.ViewClaim).FirstOrDefaultAsync();
                    if (claim != null)
                    {
                        _context.HelpdeskClaims.Remove(claim);
                        await _context.SaveChangesAsync();
                    }
                }
                if (!string.IsNullOrEmpty(tickettype.EditClaim))
                {
                    var claim = await _context.HelpdeskClaims.Where(x => x.Name == tickettype.EditClaim).FirstOrDefaultAsync();
                    if (claim != null)
                    {
                        _context.HelpdeskClaims.Remove(claim);
                        await _context.SaveChangesAsync();
                    }
                }
                if (!string.IsNullOrEmpty(tickettype.CreationClaim))
                {
                    var claim = await _context.HelpdeskClaims.Where(x => x.Name == tickettype.CreationClaim).FirstOrDefaultAsync();
                    if (claim != null)
                    {
                        _context.HelpdeskClaims.Remove(claim);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
