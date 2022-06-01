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
    public class DetailsModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DetailsModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
