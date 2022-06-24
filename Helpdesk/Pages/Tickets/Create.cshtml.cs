using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helpdesk.Data;

namespace Helpdesk.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public CreateModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TicketMaster TicketMaster { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.TicketMasters == null || TicketMaster == null)
            {
                return Page();
            }

            _context.TicketMasters.Add(TicketMaster);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
