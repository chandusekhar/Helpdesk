using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.LicenseTypes
{
    public class DeleteModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DeleteModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public LicenseType LicenseType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.LicenseType == null)
            {
                return NotFound();
            }

            var licensetype = await _context.LicenseType.FirstOrDefaultAsync(m => m.Id == id);

            if (licensetype == null)
            {
                return NotFound();
            }
            else 
            {
                LicenseType = licensetype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.LicenseType == null)
            {
                return NotFound();
            }
            var licensetype = await _context.LicenseType.FindAsync(id);

            if (licensetype != null)
            {
                LicenseType = licensetype;
                _context.LicenseType.Remove(LicenseType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
