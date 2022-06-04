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
    public class DetailsModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DetailsModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
