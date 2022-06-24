using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.Tickets
{
    public class DeleteModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DeleteModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public TicketMaster TicketMaster { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
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
