using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.Tickets
{
    public class EditModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public EditModel(Helpdesk.Data.ApplicationDbContext context)
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

            var ticketmaster =  await _context.TicketMasters.FirstOrDefaultAsync(m => m.Id == id);
            if (ticketmaster == null)
            {
                return NotFound();
            }
            TicketMaster = ticketmaster;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TicketMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketMasterExists(TicketMaster.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TicketMasterExists(string id)
        {
          return (_context.TicketMasters?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
