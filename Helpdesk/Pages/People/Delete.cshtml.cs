using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.People
{
    public class DeleteModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DeleteModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public HelpdeskUser HelpdeskUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.HelpdeskUsers == null)
            {
                return NotFound();
            }

            var helpdeskuser = await _context.HelpdeskUsers.FirstOrDefaultAsync(m => m.Id == id);

            if (helpdeskuser == null)
            {
                return NotFound();
            }
            else 
            {
                HelpdeskUser = helpdeskuser;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.HelpdeskUsers == null)
            {
                return NotFound();
            }
            var helpdeskuser = await _context.HelpdeskUsers.FindAsync(id);

            if (helpdeskuser != null)
            {
                HelpdeskUser = helpdeskuser;
                _context.HelpdeskUsers.Remove(HelpdeskUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
